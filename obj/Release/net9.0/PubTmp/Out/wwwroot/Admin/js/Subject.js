

loaddata = function () {
    debugger;
    BlockUI();
    $.ajax({
        url: "/Admin/Subject/listdata", // Gọi file PHP xử lý
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

LoadModelAdd = function () {
    debugger;
    BlockUI();
    $.ajax({
        url: "/Admin/Subject/Create", // Gọi file PHP xử lý
        type: "GET",
        success: function (response) {
            UnBlockUI();
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

LoadModelEdit = function (id) {
    debugger;
    BlockUI();
    $.ajax({
        url: "/Admin/Subject/Edit", // Gọi file PHP xử lý
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

LoadModelDetail = function (id) {
    debugger;
    BlockUI();
    $.ajax({
        url: "/Admin/Subject/Detail", // Gọi file PHP xử lý
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

successAction = function (res) {
    debugger;
    if (res.success) {
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

deleteData = function (id) {
    if (confirm("Bạn có chắc chắn muốn xóa không?")) {
        BlockUI(); // Không cho người dùng nhập liệu khi đang thao tác với dữ liệu
        $.ajax({
            url: "/Admin/Subject/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                UnBlockUI();
                loaddata();
                if (res.success) {
                    showToast(res.message);
                }
                else {
                    showToast(res.message);
                }

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
    } else {
        console.log("Hủy xóa!");
    }
}


editStatus = function (id) {

    BlockUI(); // Không cho người dùng nhập liệu khi đang thao tác với dữ liệu
    $.ajax({
        url: "/Admin/Subject/Status",
        type: "POST",
        data: { id: id },
        success: function (res) {
            UnBlockUI();
            loaddata();
            if (res.success) {
                showToast(res.message, "success");
            }
            else {
                showToast(res.message, "error");
            }
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


