# دليل تشغيل وربط منصة "بناء" (النسخة الاحترافية)

هذا الدليل يشرح كيفية تشغيل نظام "بناء" المتكامل باستخدام ASP.NET Core 8 و SQL Server.

## 1. إعداد قاعدة البيانات (SQL Server)
- قم بتنفيذ الكود الموجود في `backend/schema.sql` بالكامل.
- سيقوم السكريبت بإنشاء الجداول اللازمة (Schools, AppUsers, Events, Guests, Students, Attendances).
- تم إضافة مدرسة تجريبية ومستخدم مدير بنجاح:
    - **البريد:** `admin@hns.edu.sa`
    - **كلمة المرور:** `admin123`

## 2. إعداد الـ Backend (ASP.NET Core API)
المشروع الكامل موجود في مجلد `backend/BenaaInvitations.API/`.
- **خطوات التشغيل:**
    1. افتح المشروع في VS Code أو Visual Studio.
    2. تأكد من تحديث `appsettings.json` ببيانات الاتصال بقاعدة البيانات الخاصة بك في `BenaaConnection`.
    3. **هام جداً:** قم بتعبئة بيانات `EmailSettings` (الـ API Key لـ SendGrid أو SMTP) لتفعيل ميزة استعادة كلمة المرور.
    4. شغل المشروع باستخدام `dotnet run`.

## 3. تفعيل الربط في الواجهة الأمامية (Frontend)
في ملف `index.html` (سطر 600 تقريباً):
```javascript
const USE_API = true; // اجعلها true لتفعيل الربط الحقيقي
const API_BASE = "https://your-api-domain.com/api"; // رابط الـ API الخاص بك
```

## 4. المميزات المضافة (Security & UX)
- **Forgot Password:** نظام استعادة كلمة مرور آمن يرسل كوداً (OTP) عبر البريد الإلكتروني (`it@hns.edu.sa`) باستخدام قالب احترافي.
- **Multi-tenancy:** النظام يدعم إدارة عدة مدارس عبر نفس قاعدة البيانات.
- **MailKit Integration:** تم دمج مكتبة MailKit لإرسال رسائل بريد إلكتروني موثوقة.
- **Arabic UI:** جميع رسائل الخطأ والنجاح معربة بالكامل لتجربة مستخدم مثالية.

---
**فريق تقنية المعلومات | hns.edu.sa**
