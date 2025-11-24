namespace BTL1.BUS
{
    /// <summary>
    /// Quản lý phiên đăng nhập của nhân viên
    /// </summary>
    public static class SessionManager
    {
        public static string MaNV { get; set; }
        public static string TenNV { get; set; }
        public static string ChucVu { get; set; }
        public static string SoDienThoai { get; set; }

        /// <summary>
        /// Xóa toàn bộ thông tin phiên đăng nhập
        /// </summary>
        public static void ClearSession()
        {
            MaNV = null;
            TenNV = null;
            ChucVu = null;
            SoDienThoai = null;
        }

        /// <summary>
        /// Kiểm tra trạng thái đăng nhập
        /// </summary>
        /// <returns>True nếu đã đăng nhập</returns>
        public static bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(MaNV);
        }

        /// <summary>
        /// Kiểm tra quyền quản lý
        /// </summary>
        /// <returns>True nếu là quản lý</returns>
        public static bool IsAdmin()
        {
            return ChucVu != null && (ChucVu.Contains("Quản lý") || ChucVu.Contains("Admin") || ChucVu.Contains("Quản trị viên"));
        }

        /// <summary>
        /// Lưu thông tin đăng nhập
        /// </summary>
        public static void SetSession(string maNV, string tenNV, string chucVu, string sdt)
        {
            MaNV = maNV;
            TenNV = tenNV;
            ChucVu = chucVu;
            SoDienThoai = sdt;
        }

        /// <summary>
        /// Lấy tên hiển thị đầy đủ
        /// </summary>
        public static string GetDisplayName()
        {
            if (string.IsNullOrEmpty(TenNV))
                return "Chưa đăng nhập";

            return $"{TenNV} ({ChucVu})";
        }
        /// <summary>
        /// Kiểm tra quyền Lễ tân (Bao gồm Admin).
        /// Dùng cho các chức năng giao dịch.
        /// </summary>
        public static bool IsReceptionist()
        {
            // Admin có tất cả quyền, nếu không phải Admin thì check Chức vụ Lễ tân
            return IsAdmin() || (ChucVu != null && ChucVu.Contains("Lễ tân"));
        }
        /// Kiểm tra quyền Kế toán (Bao gồm Admin).
        /// Dùng cho chức năng Thống kê/Báo cáo.
        public static bool IsAccountant()
        {
            // Admin có tất cả quyền, nếu không phải Admin thì check Chức vụ Kế toán
            return IsAdmin() || (ChucVu != null && ChucVu.Contains("Kế toán"));
        }
    }

}