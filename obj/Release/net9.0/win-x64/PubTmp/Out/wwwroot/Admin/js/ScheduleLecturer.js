

loaddata = function () {
    debugger;
    BlockUI();
    $.ajax({
        url: "/Lecturer/Schedule/listdata", // Gọi file PHP xử lý
        type: "GET",
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



LoadModelDetail = function (id) {
    debugger;
    BlockUI();
    $.ajax({
        url: "/Lecturer/Schedule/Detail", // Gọi file PHP xử lý
        type: "GET",
        data: { id: id },
        success: function (response) {
            UnBlockUI();
            if (response.success == false) {
                showToast(response.message)
            }
            $("#target-div").html(response); // Chèn nội dung HTML vào div
            $("#myModal").modal("show"); // Hiện modal
        },
        error: function (xhr) {
            UnBlockUI();
            if (xhr.status === 401) {
                showToast("Bạn không có quyền truy cập! Vui lòng đăng nhập.");
            } else if (xhr.status === 403) {
                showToast("Bạn không có quyền thực hiện thao tác này!");
            } else {
                showToast("Không thể tải dữ liệu!");
            }
        }
    });
}


loaddata();



