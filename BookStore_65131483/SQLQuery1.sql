-- =============================
-- 1. TẠO DATABASE
-- =============================
CREATE DATABASE BookStore_65131483;
GO
USE BookStore_65131483;
GO
-- =============================
-- 1. TAIKHOAN
-- =============================
CREATE TABLE TAIKHOAN (
    MaTK INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) UNIQUE NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
    HoTen NVARCHAR(150),
    Email NVARCHAR(150) UNIQUE,
    SDT NVARCHAR(20),
    DiaChi NVARCHAR(255),
    VaiTro NVARCHAR(20) DEFAULT 'user'  -- 'user' hoặc 'admin'
);
GO

-- =============================
-- 2. TACGIA
-- =============================
CREATE TABLE TACGIA (
    MaTacGia INT IDENTITY(1,1) PRIMARY KEY,
    TenTacGia NVARCHAR(150) NOT NULL,
    QuocTich NVARCHAR(100)  NOT NULL
);
GO

-- =============================
-- 3. THELOAI
-- =============================
CREATE TABLE THELOAI (
    MaTheLoai INT IDENTITY(1,1) PRIMARY KEY,
    TenTheLoai NVARCHAR(100) UNIQUE NOT NULL
);
GO
select * from sach
delete sach
-- =============================
-- 4. SACH
-- =============================
CREATE TABLE SACH (
    MaSach INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe NVARCHAR(255) NOT NULL,
    MaTacGia INT NULL,
    MaTheLoai INT NULL,
    GiaBan DECIMAL(10,2) NOT NULL,
    SoLuongTon INT NOT NULL DEFAULT 0,
    AnhBia NVARCHAR(500),
    MoTa NVARCHAR(MAX),
	NgonNgu NVARCHAR(50),
	NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
	DaBan INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT fk_sach_tacgia FOREIGN KEY (MaTacGia) REFERENCES TACGIA(MaTacGia)
        ON DELETE NO ACTION,
    CONSTRAINT fk_sach_theloai FOREIGN KEY (MaTheLoai) REFERENCES THELOAI(MaTheLoai)
        ON DELETE NO ACTION
)


GO
select * from TAIKHOan
UPDATE TAIKHOAN
SET MatKhau = 'FF3E2A064609AC4EDCEFAFF625A2333B18993F66'
WHERE MaTK = 1;
-- =============================
-- 5. DONHANG
-- =============================
CREATE TABLE DONHANG (
    MaDH INT IDENTITY(1,1) PRIMARY KEY,
    MaTK INT NOT NULL,
    NgayDat DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) DEFAULT 'trong_gio_hang', -- trong_gio_hang, cho_xu_ly, dang_giao, hoan_thanh, huy
    DiaChiGiao NVARCHAR(255),
    TongTien DECIMAL(12,2),
    PhuongThucThanhToan NVARCHAR(50) DEFAULT 'thanh_toan_online', -- thanh_toan_online, thanh_toan_cod,
	NgayTao DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_dh_tk FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK)
);
GO


-- =============================
-- 6. CHITIETDONHANG
-- =============================
CREATE TABLE CHITIETDONHANG (
    MaCTDH INT IDENTITY(1,1) PRIMARY KEY,
    MaDH INT NOT NULL,
    MaSach INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(10,2) NOT NULL,
    CONSTRAINT fk_ctdh_dh FOREIGN KEY (MaDH) REFERENCES DONHANG(MaDH)
        ON DELETE CASCADE,
    CONSTRAINT fk_ctdh_sach FOREIGN KEY (MaSach) REFERENCES SACH(MaSach),
    CONSTRAINT uq_ctdh UNIQUE (MaDH, MaSach)
);
GO



-- =============================
-- 7. DANHGIA
-- =============================
CREATE TABLE DANHGIA (
    MaDG INT IDENTITY(1,1) PRIMARY KEY,
    MaTK INT NOT NULL,
    MaSach INT NOT NULL,
	MaDH INT NOT NULL,
    Diem TINYINT NOT NULL CHECK (Diem BETWEEN 1 AND 5),
    BinhLuan NVARCHAR(MAX),
    NgayDanhGia DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_dg_tk FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK)
        ON DELETE CASCADE,
    CONSTRAINT fk_dg_sach FOREIGN KEY (MaSach) REFERENCES SACH(MaSach)
        ON DELETE CASCADE,
		 CONSTRAINT fk_dg_DH FOREIGN KEY (MaDH) REFERENCES DONHANG(MaDH)
        ON DELETE CASCADE,
    CONSTRAINT uq_dg UNIQUE (MaTK, MaDH, MaSacH)
);


