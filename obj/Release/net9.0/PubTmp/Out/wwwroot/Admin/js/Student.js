
function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return "";
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

loaddata = function () {
    debugger;
    BlockUI();

    var classId = getParameterByName("ClassId"); // Lấy classId từ URL

    $.ajax({
        url: "/Admin/Student/listdata",
        type: "GET",
        data: { ClassId: encodeURIComponent(classId) },
        success: function (response) {
            UnBlockUI();
            $("#listdata").html(response);
        },
        error: function (xhr) {
            UnBlockUI();
            if (xhr.status === 401) {
                showToast("Bạn không có quyền truy cập! Vui lòng đăng nhập.");
                setTimeout(() => {
                    window.history.back();
                }, 2000);
            } else if (xhr.status === 403) {
                showToast("Bạn không có quyền thực hiện thao tác này!");
            } else {
                showToast("Không thể tải dữ liệu!");
            }
        }
    });
};

// Gọi tự động khi trang tải xong
$(document).ready(function () {
    loaddata();
});



LoadModelAdd = function () {
    debugger;
    BlockUI();
    $.ajax({
        url: "/Admin/Student/Create", // Gọi file PHP xử lý
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
        url: "/Admin/Student/Edit", // Gọi file PHP xử lý
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
        url: "/Admin/Student/Detail", // Gọi file PHP xử lý
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
            url: "/Admin/Student/Delete",
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
        url: "/Admin/Student/Status",
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
function LoadModeCreateAcount() {
    let values = $("input[name='chk']:checked").map(function () {
        return this.value;
    }).get().join(","); // Nối thành chuỗi "A,B,C"
    BlockUI();
    $.ajax({
        url: "/Admin/Student/CreateAccount",
        type: "Get",
        data: { selectedValues: values },
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

