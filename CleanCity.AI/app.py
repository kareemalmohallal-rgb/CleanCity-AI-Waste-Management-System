# ==============================================
# CleanCity AI Service
# Python FastAPI + YOLOv8
# ==============================================

from fastapi import FastAPI, UploadFile, File, HTTPException
from ultralytics import YOLO
from PIL import Image
import io
import os

# -----------------------------
# تهيئة التطبيق
# -----------------------------
app = FastAPI(title="CleanCity AI Service")

# -----------------------------
# إعدادات النموذج
# -----------------------------
MODEL_PATH = r"C:\Users\Owner\Desktop\the_last_version_of_the_project\CleanCity-AI-Service\weights\best.pt"  # مسار نموذج YOLO المدرب
CONF_THRESHOLD = 0.60  # الحد الأدنى للثقة لقبول الكشف
GARBAGE_CLASS_NAME = "garbage"  # اسم الفئة التي نبحث عنها

# التحقق من وجود النموذج
if not os.path.exists(MODEL_PATH):
    raise RuntimeError(f"Model not found: {MODEL_PATH}")

# تحميل النموذج
model = YOLO(MODEL_PATH)

# ==============================================
# Health Check Endpoint
# ----------------------------------------------
# endpoint لفحص جاهزية الخدمة والنموذج
# ==============================================
@app.get("/health")
def health():
    """
    يتحقق من أن الخدمة تعمل وأن النموذج موجود
    """
    return {
        "status": "ok",
        "model_path": MODEL_PATH
    }

# ==============================================
# AI Prediction Endpoint
# ----------------------------------------------
# endpoint لتحليل الصور واكتشاف المخلفات
# ==============================================
@app.post("/predict-waste")
async def predict_waste(file: UploadFile = File(...)):
    """
    يستقبل صورة واحدة ويقوم بتحليلها باستخدام نموذج YOLO
    """
    try:
        # -----------------------------
        # قراءة محتوى الصورة
        # -----------------------------
        content = await file.read()
        if not content:
            raise HTTPException(status_code=400, detail="Empty file.")

        # تحويل البايت إلى صورة PIL
        image = Image.open(io.BytesIO(content)).convert("RGB")

        # -----------------------------
        # تشغيل النموذج على الصورة
        # -----------------------------
        results = model.predict(
            source=image,
            conf=CONF_THRESHOLD,
            verbose=False
        )

        result = results[0]  # نأخذ نتيجة الصورة الأولى فقط
        boxes = result.boxes  # مربعات الكشف

        # -----------------------------
        # تحليل النتائج
        # -----------------------------
        garbage_detected = False
        best_confidence = 0.0
        detections = []

        if boxes is not None and len(boxes) > 0:
            for box in boxes:
                # استخراج فئة الكائن والـ confidence
                cls_id = int(box.cls[0].item())
                conf = float(box.conf[0].item())
                class_name = result.names.get(cls_id, str(cls_id))

                # إضافة الكشف إلى القائمة
                detections.append({
                    "class_id": cls_id,
                    "class_name": class_name,
                    "confidence": round(conf, 4)
                })

                # التحقق من اكتشاف المخلفات
                if class_name == GARBAGE_CLASS_NAME and conf >= CONF_THRESHOLD:
                    garbage_detected = True
                    if conf > best_confidence:
                        best_confidence = conf

        # -----------------------------
        # إرجاع النتيجة في JSON
        # -----------------------------
        return {
            "isGarbageDetected": garbage_detected,  # هل تم اكتشاف المخلفات؟
            "confidencePercentage": round(best_confidence * 100, 2),  # أعلى نسبة ثقة
            "rejectionReason": None if garbage_detected else "AI model did not detect garbage in the image.",
            "detections": detections  # جميع الكائنات المكتشفة
        }

    except HTTPException:
        # إعادة رفع أخطاء HTTP مباشرة
        raise
    except Exception as ex:
        # أي خطأ آخر يتم تحويله إلى HTTP 500
        raise HTTPException(status_code=500, detail=str(ex))