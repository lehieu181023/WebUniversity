
loaddata = function () {
    $.ajax({
        url: "GioHang/listdata", // Gọi file PHP xử lý
        type: "GET",
        success: function (response) {
            $("#listdata").html(response); // Chèn dữ liệu vào bảng
            if (response.success == false) {
                alert(response.message);
            }
        },
        error: function () {
            alert("Không thể tải dữ liệu!");
        }
    });
}
deleteData = function (id) {
    if (confirm("Bạn có chắc chắn muốn xóa không?")) {
        BlockUI(); // Không cho người dùng nhập liệu khi đang thao tác với dữ liệu
        $.ajax({
            url: "GioHang/DeleteFromCart",
            type: "POST",
            data: { id: id },
            success: function (res) {
                UnBlockUI();
                loaddata();
                if (res.success) {

                }
                else {
                    alert(res.message);
                }

            },
            error: function () {
                UnBlockUI();
                alert("Không thể xóa dữ liệu!");
            }
        });
    } else {
        console.log("Hủy xóa!");
    }
}

editCart = function (id, quantity) {
    if (quantity == null || quantity == "") {
        quantity = 1;
    }
    BlockUI(); // Không cho người dùng nhập liệu khi đang thao tác với dữ liệu
    $.ajax({
        url: "GioHang/EditCart",
        type: "POST",
        data: { id: id, quantity: quantity },
        success: function (res) {
            UnBlockUI();
            loaddata();
            if (res.success === false) {
                alert(res.message);
            }
        },
        error: function () {
            UnBlockUI();
            alert("Không thay đổi sản phẩm trong giỏ hàng!");
        }
    });
}

