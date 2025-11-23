-- Script tạo bảng Review/Rating cho sản phẩm
USE [BSDB2]
GO

-- Tạo bảng đánh giá sản phẩm
CREATE TABLE [dbo].[DanhGia](
	[MaDanhGia] [int] IDENTITY(1,1) NOT NULL,
	[MaSach] [int] NOT NULL,
	[MaKH] [int] NOT NULL,
	[SoSao] [int] NOT NULL CHECK ([SoSao] >= 1 AND [SoSao] <= 5),
	[TieuDe] [nvarchar](200) NULL,
	[NoiDung] [nvarchar](1000) NULL,
	[NgayDanhGia] [datetime] NULL DEFAULT(GETDATE()),
	[TrangThai] [bit] NULL DEFAULT(1), -- 1: Hiển thị, 0: Ẩn
	[HuuIch] [int] NULL DEFAULT(0), -- Số lượt "Hữu ích"
	PRIMARY KEY CLUSTERED ([MaDanhGia] ASC),
	FOREIGN KEY([MaSach]) REFERENCES [dbo].[Sach] ([MaSach]) ON DELETE CASCADE,
	FOREIGN KEY([MaKH]) REFERENCES [dbo].[KhachHang] ([MaKH])
)
GO

-- Tạo index để tăng tốc truy vấn
CREATE INDEX IX_DanhGia_MaSach ON [dbo].[DanhGia]([MaSach])
GO
CREATE INDEX IX_DanhGia_MaKH ON [dbo].[DanhGia]([MaKH])
GO

-- Thêm cột Rating trung bình và số lượt đánh giá vào bảng Sach
ALTER TABLE [dbo].[Sach]
ADD [DiemTrungBinh] [decimal](3,2) NULL DEFAULT(0),
    [SoLuotDanhGia] [int] NULL DEFAULT(0)
GO

-- Tạo trigger tự động cập nhật điểm trung bình khi có đánh giá mới
CREATE TRIGGER trg_UpdateRating
ON [dbo].[DanhGia]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Cập nhật cho sách bị ảnh hưởng từ INSERT/UPDATE
    IF EXISTS(SELECT * FROM inserted)
    BEGIN
        UPDATE s
        SET 
            s.DiemTrungBinh = ISNULL((SELECT AVG(CAST(dg.SoSao AS DECIMAL(3,2))) 
                                      FROM DanhGia dg 
                                      WHERE dg.MaSach = s.MaSach AND dg.TrangThai = 1), 0),
            s.SoLuotDanhGia = (SELECT COUNT(*) 
                               FROM DanhGia dg 
                               WHERE dg.MaSach = s.MaSach AND dg.TrangThai = 1)
        FROM Sach s
        INNER JOIN inserted i ON s.MaSach = i.MaSach;
    END

    -- Cập nhật cho sách bị ảnh hưởng từ DELETE
    IF EXISTS(SELECT * FROM deleted) AND NOT EXISTS(SELECT * FROM inserted)
    BEGIN
        UPDATE s
        SET 
            s.DiemTrungBinh = ISNULL((SELECT AVG(CAST(dg.SoSao AS DECIMAL(3,2))) 
                                      FROM DanhGia dg 
                                      WHERE dg.MaSach = s.MaSach AND dg.TrangThai = 1), 0),
            s.SoLuotDanhGia = (SELECT COUNT(*) 
                               FROM DanhGia dg 
                               WHERE dg.MaSach = s.MaSach AND dg.TrangThai = 1)
        FROM Sach s
        INNER JOIN deleted d ON s.MaSach = d.MaSach;
    END
END
GO

-- Thêm dữ liệu mẫu (đánh giá cho một số sách)
INSERT INTO [dbo].[DanhGia] ([MaSach], [MaKH], [SoSao], [TieuDe], [NoiDung], [NgayDanhGia], [TrangThai])
VALUES 
-- Đánh giá cho sách MaSach = 1 (Me Before You)
(1, 1, 5, N'Cuốn sách tuyệt vời!', N'Câu chuyện cảm động, viết rất hay. Đọc xong không cầm được nước mắt. Rất đáng đọc!', DATEADD(day, -10, GETDATE()), 1),
(1, 7, 4, N'Khá hay', N'Nội dung hay nhưng hơi dài dòng ở một số chỗ. Nhìn chung vẫn rất đáng đọc.', DATEADD(day, -8, GETDATE()), 1),

-- Đánh giá cho sách MaSach = 10 (Ngày xưa có một chuyện tình)
(10, 1, 5, N'Tuyệt phẩm của Nguyễn Nhật Ánh', N'Đọc đi đọc lại nhiều lần vẫn không chán. Cảm xúc rất chân thật.', DATEADD(day, -5, GETDATE()), 1),
(10, 7, 5, N'Hay quá!', N'Mình đã khóc khi đọc cuốn này. Recommend cho tất cả mọi người!', DATEADD(day, -3, GETDATE()), 1),
(10, 10, 4, N'Đáng đọc', N'Viết hay, nội dung sâu sắc. Giá hơi cao nhưng xứng đáng.', DATEADD(day, -1, GETDATE()), 1),

-- Đánh giá cho sách MaSach = 8 (Combo Cứ Bình Tĩnh)
(8, 7, 4, N'Sách hay cho người trẻ', N'Nội dung phù hợp với lứa tuổi thanh niên. Nhiều bài học hay.', DATEADD(day, -7, GETDATE()), 1),

-- Đánh giá cho sách MaSach = 13 (Dám Làm Giàu)
(13, 10, 3, N'Tạm được', N'Có một số nội dung hay nhưng không mới lắm. Có thể tham khảo.', DATEADD(day, -2, GETDATE()), 1)
GO

PRINT 'Đã tạo bảng DanhGia và dữ liệu mẫu thành công!'
PRINT 'Trigger tự động cập nhật điểm rating đã được tạo!'
GO
