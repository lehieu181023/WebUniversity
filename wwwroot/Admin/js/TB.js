

loadTB = function () {
    debugger;
    BlockUI();
    $.ajax({
        url: "/Admin/DashBoard/AccountTB", // Gọi file PHP xử lý
        type: "GET",

        success: function (response) {
            UnBlockUI();
            $("#TB").html(response); // Chèn dữ liệu vào bảng
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

changePass = function (Id) {
    debugger;
    BlockUI();
    $.ajax({
        url: "/Admin/DashBoard/EditPassword", // Gọi file PHP xử lý
        type: "GET",
        data: { id: Id },
        success: function (response) {
            UnBlockUI();
            $("#target-div-changepass").html(response); // Chèn nội dung HTML vào div
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


successChange = function (res) {
    debugger;
    if (res.success == true) {
        UnBlockUI();
        $('#btnclosemodel').click();
        loaddata();
        showToast(res.message, "success");
    }
    else {
        UnBlockUI();
        showToast(res.message, "error");
    }
}



loadTB();



