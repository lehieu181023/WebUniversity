loaduserstatus = function () {
    $.ajax({
        url: "/Home/UserStatus", // Gọi file PHP xử lý
        type: "POST",
        success: function (response) {
            if (response.success == false) {
                alert(response.message);
                window.location.href = "/Account/Logout";
            }
            else {
                $("#userstatus").html(response); // Chèn dữ liệu vào bảng
            }
        },
        error: function () {
            alert("Không thể tải dữ liệu!");
        }
    });
}

AddToCart = function (id, quantity) {
    BlockUI(); // Không cho người dùng nhập liệu khi đang thao tác với dữ liệu
    if (quantity == null || quantity == "") {
        quantity = 1;
    }
    $.ajax({
        url: "/GioHang/AddToCart",
        type: "POST",
        data: { id: id, quantity: quantity },
        success: function (res) {
            UnBlockUI();
            if (res.success) {
                alert(res.message);
            }
            else {
                alert(res.message);
            }
        },
        error: function () {
            UnBlockUI();
            alert("Không thể thêm sản phẩm vào giỏ hàng!");
        }
    });
}

loaduserstatus();
