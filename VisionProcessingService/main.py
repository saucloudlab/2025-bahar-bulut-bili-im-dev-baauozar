from fastapi import FastAPI
from pydantic import BaseModel
import cv2
import numpy as np
import requests

app = FastAPI(title="Real Agri-Stress Vision API")

class ImageRequest(BaseModel):
    imageUrl: str

def analyze_leaf_stress(image_url: str):
    try:
        # هنا أضفنا القناع (User-Agent) عشان نتخطى حماية المواقع
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
        }
        
        # 1. تحميل الصورة من الرابط مع القناع
        resp = requests.get(image_url, headers=headers, timeout=10)
        resp.raise_for_status()
        
        image_array = np.asarray(bytearray(resp.content), dtype=np.uint8)
        img = cv2.imdecode(image_array, cv2.IMREAD_COLOR)

        if img is None:
            return "Error", "Could not decode image."

        # 2. تحويل الصورة إلى نطاق ألوان HSV لسهولة فصل الألوان
        hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)

        # 3. تحديد نطاقات الألوان (الأخضر الصحي مقابل الأصفر/البني المجهد)
        lower_green = np.array([35, 40, 40])
        upper_green = np.array([85, 255, 255])
        
        lower_yellow_brown = np.array([10, 40, 40])
        upper_yellow_brown = np.array([34, 255, 255])

        # 4. حساب عدد البيكسلات لكل لون
        green_mask = cv2.inRange(hsv, lower_green, upper_green)
        yellow_mask = cv2.inRange(hsv, lower_yellow_brown, upper_yellow_brown)

        green_pixels = cv2.countNonZero(green_mask)
        yellow_pixels = cv2.countNonZero(yellow_mask)
        total_leaf_pixels = green_pixels + yellow_pixels

        if total_leaf_pixels == 0:
            return "Unknown", "No plant detected in the image."

        # 5. خوارزمية التحليل وإصدار التبرير (XAI Justification)
        stress_ratio = yellow_pixels / total_leaf_pixels

        if stress_ratio > 0.5:
            level = "High"
            justification = f"Computer Vision Analysis: Detected {stress_ratio*100:.1f}% stressed (yellow/brown) pixels. Severe chlorosis detected."
        elif stress_ratio > 0.2:
            level = "Moderate"
            justification = f"Computer Vision Analysis: Detected {stress_ratio*100:.1f}% stressed pixels. Early signs of nutrient deficiency."
        else:
            level = "Low"
            justification = f"Computer Vision Analysis: Leaf is healthy. Only {stress_ratio*100:.1f}% abnormal pixels found."

        return level, justification

    except Exception as e:
        return "Error", f"Failed to process image: {str(e)}"

@app.post("/api/analyze")
async def analyze_image(request: ImageRequest):
    level, justification = analyze_leaf_stress(request.imageUrl)

    return {
        "status": "Completed" if level != "Error" else "Failed",
        "stressLevel": level,
        "xaiResult": justification,
        "processedImage": request.imageUrl 
    }

@app.get("/health")
async def health_check():
    return {"status": "Healthy", "service": "Real Vision Processing with OpenCV"}