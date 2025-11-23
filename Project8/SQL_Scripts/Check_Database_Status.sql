-- Script kiểm tra và sửa lỗi Foreign Key
USE [BSDB2]
GO

-- 1. Kiểm tra bảng Voucher đã tồn tại chưa
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Voucher')
BEGIN
    PRINT 'CẢNH BÁO: Bảng Voucher chưa tồn tại!'
    PRINT 'Vui lòng chạy file: Create_Voucher_Tables.sql trước'
END
ELSE
    PRINT 'OK: Bảng Voucher đã tồn tại'
GO

-- 2. Xóa Foreign Key cũ nếu có (để tránh lỗi duplicate)
DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name 
FROM sys.foreign_keys 
WHERE parent_object_id = OBJECT_ID('DonDatHang') 
  AND referenced_object_id = OBJECT_ID('Voucher')

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [dbo].[DonDatHang] DROP CONSTRAINT ' + @ConstraintName)
    PRINT 'Đã xóa Foreign Key cũ: ' + @ConstraintName
END
GO

-- 3. Thêm Foreign Key mới (nếu bảng Voucher đã tồn tại)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Voucher')
BEGIN
    -- Đặt NULL cho các MaVoucher không hợp lệ
    UPDATE DonDatHang 
    SET MaVoucher = NULL 
    WHERE MaVoucher IS NOT NULL 
      AND MaVoucher NOT IN (SELECT MaVoucher FROM Voucher)
    
    -- Thêm Foreign Key
    ALTER TABLE [dbo].[DonDatHang]
    ADD CONSTRAINT FK_DonDatHang_Voucher 
    FOREIGN KEY([MaVoucher]) REFERENCES [dbo].[Voucher] ([MaVoucher])
    
    PRINT 'Đã thêm Foreign Key: FK_DonDatHang_Voucher'
END
GO

-- 4. Kiểm tra tất cả các cột cần thiết
DECLARE @MissingColumns TABLE (ColumnName NVARCHAR(50))

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DonDatHang') AND name = 'ThanhToan')
    INSERT INTO @MissingColumns VALUES ('ThanhToan')

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DonDatHang') AND name = 'Tracking')
    INSERT INTO @MissingColumns VALUES ('Tracking')

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DonDatHang') AND name = 'MaVoucher')
    INSERT INTO @MissingColumns VALUES ('MaVoucher')

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DonDatHang') AND name = 'GiamGia')
    INSERT INTO @MissingColumns VALUES ('GiamGia')

IF EXISTS (SELECT * FROM @MissingColumns)
BEGIN
    PRINT '-------------------------------------------'
    PRINT 'CẢNH BÁO: Các cột sau còn thiếu:'
    SELECT * FROM @MissingColumns
    PRINT 'Vui lòng chạy file: Fix_DonDatHang_Columns.sql'
    PRINT '-------------------------------------------'
END
ELSE
    PRINT 'OK: Tất cả các cột cần thiết đã có'
GO

-- 5. Hiển thị cấu trúc bảng DonDatHang
PRINT ''
PRINT '========================================='
PRINT 'CẤU TRÚC BẢNG DonDatHang:'
PRINT '========================================='
SELECT 
    c.name AS [Column Name],
    t.name AS [Data Type],
    c.max_length AS [Max Length],
    c.is_nullable AS [Nullable]
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('DonDatHang')
ORDER BY c.column_id
GO

PRINT ''
PRINT 'Kiểm tra hoàn tất!'
