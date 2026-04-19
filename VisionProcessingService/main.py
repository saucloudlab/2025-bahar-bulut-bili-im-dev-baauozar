from fastapi import FastAPI
from pydantic import BaseModel
import cv2
import numpy as np
import requests
import base64 # <-- المكتبة الجديدة لتشفير الصورة
from typing import Tuple

app = FastAPI(title="Agri-Stress XAI Vision API")

class ImageRequest(BaseModel):
    imageUrl: str

def analyze_leaf_stress(image_url: str) -> Tuple[str, str, str]:
    try:
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
        }
        
        resp = requests.get(image_url, headers=headers, timeout=10)
        resp.raise_for_status()
        
        image_array = np.asarray(bytearray(resp.content), dtype=np.uint8)
        img = cv2.imdecode(image_array, cv2.IMREAD_COLOR)

        if img is None:
            return "Error", "Could not decode image from the provided URL.", ""

        hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)

        lower_green = np.array([35, 40, 40])
        upper_green = np.array([85, 255, 255])
        
        lower_stress = np.array([10, 40, 40])
        upper_stress = np.array([34, 255, 255])

        green_mask = cv2.inRange(hsv, lower_green, upper_green)
        stress_mask = cv2.inRange(hsv, lower_stress, upper_stress)

        green_count = cv2.countNonZero(green_mask)
        stress_count = cv2.countNonZero(stress_mask)
        total_pixels = green_count + stress_count

        if total_pixels == 0:
            return "Unknown", "No significant plant material detected in the image.", ""

        # --- هنا السحر حق الـ XAI البصري ---
        # 1. نأخذ نسخة من الصورة الأصلية
        xai_image = img.copy()
        
        # 2. نلون البكسلات المريضة باللون الأحمر (BGR = [0, 0, 255])
        xai_image[stress_mask > 0] = [0, 0, 255]
        
        # 3. نحول الصورة الناتجة إلى نص مشفر (Base64)
        _, buffer = cv2.imencode('.jpg', xai_image)
        visual_explanation_base64 = base64.b64encode(buffer).decode('utf-8')
        # -----------------------------------

        stress_ratio = stress_count / total_pixels

        if stress_ratio > 0.5:
            level = "High"
            justification = f"XAI Analysis: Severe stress detected. {stress_ratio*100:.1f}% of the leaf shows yellowing/browning."
        elif stress_ratio > 0.15:
            level = "Moderate"
            justification = f"XAI Analysis: Moderate stress. {stress_ratio*100:.1f}% of the leaf area is affected."
        else:
            level = "Low"
            justification = f"XAI Analysis: Healthy leaf. Only {stress_ratio*100:.1f}% abnormal pixels detected."

        # نرجع الصورة المشفرة مع النتيجة
        return level, justification, visual_explanation_base64

    except Exception as e:
        return "Error", f"Processing failed: {str(e)}", ""

@app.post("/analyze")
async def analyze_image(request: ImageRequest):
    # نستقبل المتغير الثالث (الصورة)
    level, justification, xai_image_b64 = analyze_leaf_stress(request.imageUrl)

    return {
        "status": "Completed" if level != "Error" else "Failed",
        "stressLevel": level,
        "xaiResult": justification,
        "visualExplanation": xai_image_b64  # <-- نضيفها في الـ JSON اللي يرجع
    }

@app.get("/health")
async def health():
    return {"status": "Healthy", "service": "Vision-XAI"}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)