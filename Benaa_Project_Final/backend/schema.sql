-- ============================================================
-- BENAA PLATFORM - DATABASE SCHEMA (SQL SERVER)
-- ============================================================

-- 1. Schools Table (Multi-tenancy Support)
CREATE TABLE [dbo].[Schools] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(255) NOT NULL,
    [Subdomain] NVARCHAR(100) NOT NULL UNIQUE,
    [PrimaryColor] NVARCHAR(20) DEFAULT '#2563eb'
);

-- 2. AppUsers Table (Authentication & Security)
CREATE TABLE [dbo].[AppUsers] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [SchoolId] INT NOT NULL FOREIGN KEY REFERENCES [Schools](Id),
    [Name] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [Role] NVARCHAR(50) DEFAULT 'Viewer', -- Viewer, Admin, Scanner
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    [IsActive] BIT DEFAULT 1,
    
    -- Password Reset Fields
    [ResetPasswordToken] NVARCHAR(100) NULL,
    [ResetTokenExpiry] DATETIME NULL
);

-- 3. Students Table (Permanent Records)
CREATE TABLE [dbo].[Students] (
    [NationalId] NVARCHAR(50) PRIMARY KEY, -- Student/Resident ID
    [SchoolId] INT NOT NULL FOREIGN KEY REFERENCES [Schools](Id),
    [NameAr] NVARCHAR(255) NOT NULL,
    [NameEn] NVARCHAR(255) NULL,
    [Email] NVARCHAR(255) NULL,
    [Phone] NVARCHAR(20) NULL,
    [ParentName] NVARCHAR(255) NULL,
    [Grade] NVARCHAR(100) NULL,
    [EducationLevel] NVARCHAR(100) NULL,
    [Section] NVARCHAR(100) NULL,
    [AcademicYear] NVARCHAR(50) NULL
);

-- 4. Events Table
CREATE TABLE [dbo].[Events] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [SchoolId] INT NOT NULL FOREIGN KEY REFERENCES [Schools](Id),
    [Title] NVARCHAR(255) NOT NULL,
    [Subtitle] NVARCHAR(255) NULL,
    [EventDate] DATETIME NOT NULL,
    [Location] NVARCHAR(MAX) NULL,
    [LocationUrl] NVARCHAR(MAX) NULL, -- Google Maps
    [Description] NVARCHAR(MAX) NULL,
    [LogoUrl] NVARCHAR(MAX) NULL,
    [ThemeColor] NVARCHAR(20) NULL,
    [AttachmentUrl] NVARCHAR(MAX) NULL, -- Invitation Image/Design
    [CustomTemplatePath] NVARCHAR(MAX) NULL,
    [NameX] INT NULL,
    [NameY] INT NULL
);

-- 5. Guests Table (Invitation Tracking)
CREATE TABLE [dbo].[Guests] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [SchoolId] INT NOT NULL FOREIGN KEY REFERENCES [Schools](Id),
    [EventId] INT NOT NULL FOREIGN KEY REFERENCES [Events](Id),
    [Name] NVARCHAR(255) NOT NULL,
    [Phone] NVARCHAR(20) NOT NULL,
    [Grade] NVARCHAR(100) NULL,
    [Section] NVARCHAR(100) NULL,
    [Status] INT DEFAULT 0, -- 0=Pending, 1=Confirmed, 2=Declined
    [SecureToken] NVARCHAR(100) NOT NULL UNIQUE, -- QR Code Key
    [RSVPDate] DATETIME NULL,
    [IsNotified] BIT DEFAULT 0
);

-- 6. Attendances Table (Scanning History)
CREATE TABLE [dbo].[Attendances] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [SchoolId] INT NOT NULL FOREIGN KEY REFERENCES [Schools](Id),
    [GuestId] INT NOT NULL FOREIGN KEY REFERENCES [Guests](Id),
    [EventId] INT NOT NULL FOREIGN KEY REFERENCES [Events](Id),
    [ScanTime] DATETIME DEFAULT GETDATE(),
    [ScannedBy] NVARCHAR(255) NULL
);

-- ============================================================
-- INITIAL SEED DATA (FOR TESTING)
-- ============================================================

-- Insert Schools
INSERT INTO [Schools] (Name, Subdomain, PrimaryColor) 
VALUES (N'مدارس الأحساء النموذجية الأهلية', 'ahsa-model', '#1e293b');

-- Insert Initial Admin (Password for demo is 'admin123' - Hashed in production)
-- In realistic app, use a proper Hashing tool. 
-- For now we use plain text for identification in SSMS scripts if needed, but the API should hash it.
INSERT INTO [AppUsers] (SchoolId, Name, Email, PasswordHash, Role) 
VALUES (1, N'أحمد طلعت', 'admin@hns.edu.sa', 'admin123', 'Admin');

INSERT INTO [AppUsers] (SchoolId, Name, Email, PasswordHash, Role) 
VALUES (1, N'مشرف بوابة 1', 'scanner@hns.edu.sa', 'scanner123', 'Scanner');
