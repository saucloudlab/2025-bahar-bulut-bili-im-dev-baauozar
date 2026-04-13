FROM python:3.11-slim
WORKDIR /app

# نسخ وتثبيت المكتبات
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# نسخ كود التطبيق
COPY . .
EXPOSE 8000

# تشغيل الخادم
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]