
loaddata = function (page,MaDm) {
    if (page == null) { page = 1 }

    $.ajax({
        url: "CuaHang/listdata", // Gọi file PHP xử lý
        type: "POST",
        data: { page: page, MaDm: MaDm }, 
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

loaddata(1);

