loadReport1 = function () {
    BlockUI();
    $.ajax({
        url: "/Admin/DashBoard/Report1",
        type: "GET",
        success: function (res) {
            UnBlockUI();
            debugger;
            if (res.success) {
                var data = res.data;

                // Trích xuất dữ liệu
                var years = data.years;
                var studentCounts = data.students;
                var lecturerCounts = data.lecturers;
                var roomCounts = data.rooms;

                // Cập nhật biểu đồ
                new ApexCharts(document.querySelector("#reportsChart"), {
                    series: [
                        {
                            name: 'Students',
                            data: [2000,3560,2300,4000,3560]
                        },
                        {
                            name: 'Lecturers',
                            data: [50,46,51,66,60]
                        },
                        {
                            name: 'Rooms',
                            data: [100,150,200,210,300]
                        }
                    ],
                    chart: {
                        height: 350,
                        type: 'area',
                        toolbar: {
                            show: false
                        },
                    },
                    markers: {
                        size: 4
                    },
                    colors: ['#4154f1', '#2eca6a', '#ff771d'], // Màu cho từng series
                    fill: {
                        type: "gradient",
                        gradient: {
                            shadeIntensity: 1,
                            opacityFrom: 0.3,
                            opacityTo: 0.4,
                            stops: [0, 90, 100]
                        }
                    },
                    dataLabels: {
                        enabled: false
                    },
                    stroke: {
                        curve: 'smooth',
                        width: 2
                    },
                    xaxis: {
                        categories: years, // Danh sách năm từ API
                        title: {
                            text: "Year"
                        }
                    },
                    tooltip: {
                        x: {
                            format: 'yyyy' // Hiển thị năm đúng format
                        },
                    }
                }).render();
            }
            else {
                showToast("lỗi thống kê","error");
            }
        },
        error: function (xhr) {
            UnBlockUI();
            showToast("lỗi tải dữ liệu", "error");
        }
    });
}

loadReport1();
