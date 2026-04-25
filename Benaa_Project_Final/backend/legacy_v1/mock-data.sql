-- Mock Data for benaa Invitations (SQLite)

-- 1. Create initial event
INSERT INTO Events (Title, Subtitle, EventDate, Location, Description, SchoolName, LogoUrl, ThemeColor)
VALUES (
    'حفل تخرج الصفوف العليا', 
    'الدفعة العاشرة - مسار متميز', 
    '2026-05-15 19:00:00', 
    'مسرح مجمع مدارس الأحساء النموذجية', 
    'نتشرف بدعوتكم لحضور حفل تخرج أبنائكم وبناتكم للمرحلة الابتدائية والمتوسطة.', 
    'مدارس الأحساء النموذجية الأهلية', 
    'school-logo.png', 
    '#6366f1'
);

-- 2. Add sample guests
INSERT INTO Guests (Name, Phone, Grade, Section, EventId, Status, SecureToken, IsNotified)
VALUES 
('أحمد محمد طاهر طارق', '966500123456', 'الخامس الابتدائي', 'أ', 1, 0, 'TOKEN_A1B2C3', 0),
('خالد العتيبي', '966501112223', 'السادس الابتدائي', 'ب', 1, 1, 'TOKEN_D4E5F6', 1),
('ياسر الرميكي', '966502223334', 'الرابع الابتدائي', 'ج', 1, 2, 'TOKEN_G7H8I9', 0);

-- 3. Add sample attendance scan
INSERT INTO Attendances (GuestId, EventId, ScanTime, ScannedBy)
VALUES (2, 1, '2026-05-15 19:15:00', 'موظف الأمن - بوابة 1');
