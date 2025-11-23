ALTER TABLE [dbo].[KhachHang]
ADD [ResetPasswordToken] NVARCHAR(100) NULL,
    [ResetPasswordExpiry] DATETIME NULL;
