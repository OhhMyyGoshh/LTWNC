-- Script để thêm trường OTP vào bảng KhachHang
-- Chạy script này để thêm 2 cột: OTPCode và OTPExpiry

USE [YourDatabaseName] -- Thay đổi tên database của bạn
GO

-- Kiểm tra xem cột đã tồn tại chưa, nếu chưa thì thêm
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[KhachHang]') AND name = 'OTPCode')
BEGIN
    ALTER TABLE [dbo].[KhachHang]
    ADD [OTPCode] NVARCHAR(10) NULL;
    PRINT 'Đã thêm cột OTPCode';
END
ELSE
BEGIN
    PRINT 'Cột OTPCode đã tồn tại';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[KhachHang]') AND name = 'OTPExpiry')
BEGIN
    ALTER TABLE [dbo].[KhachHang]
    ADD [OTPExpiry] DATETIME NULL;
    PRINT 'Đã thêm cột OTPExpiry';
END
ELSE
BEGIN
    PRINT 'Cột OTPExpiry đã tồn tại';
END
GO

PRINT 'Hoàn thành! Bảng KhachHang đã có đầy đủ trường OTP.';
GO

