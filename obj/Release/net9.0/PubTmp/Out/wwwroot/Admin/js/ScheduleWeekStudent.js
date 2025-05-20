

loaddata = function (week) {
    if (week == null) {
        week = 1;
    }
    debugger;
    BlockUI();
    $.ajax({
        url: "/Student/ScheduleWeek/listdata", // Gọi file PHP xử lý
        type: "GET",
        data: { week: week },
        success: function (response) {
            UnBlockUI();
            $("#listdata").html(response); // Chèn dữ liệu vào bảng
        },
        error: function (xhr) {
            UnBlockUI();
            if (xhr.status === 401) {
                showToast("Bạn không có quyền truy cập! Vui lòng đăng nhập.");
                setTimeout(() => {
                    window.history.back(); // Quay lại trang trước
                }, 2000); // Đợi 2 giây để hiển thị thông báo rồi quay lại
            } else if (xhr.status === 403) {
                showToast("Bạn không có quyền thực hiện thao tác này!");
            } else {
                showToast("Không thể tải dữ liệu!");
            }
        }
    });
}



loaddata();



