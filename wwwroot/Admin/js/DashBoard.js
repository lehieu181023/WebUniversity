
loadReport1 = function () {
    BlockUI();
    $.ajax({
        url: "/Admin/DashBoard/Report",
        type: "GET",
        success: function (res) {
            UnBlockUI();
            var data = res.data;
            var faculty = data.faculty;
            var lecturerNam = data.lecturerNam;
            var lecturerNu = data.lecturerNu;
            debugger;
            if (res.success) {
                Highcharts.chart('reportsChart', {
                    chart: {
                        type: 'column'
                    },
                    title: {
                        text: 'Số lượng giảng viên theo khoa'
                    },
                    xAxis: {
                        categories: faculty, // Danh sách tên khoa từ API
                        title: {
                            text: 'Khoa'
                        }
                    },
                    tooltip: {
                        format: '<b>{key}</b><br/>{series.name}: {y}<br/>' +
                            'Total: {point.stackTotal}'
                    },

                    plotOptions: {
                        column: {
                            stacking: 'normal'
                        }
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Số lượng giảng viên'
                        }
                    },
                    series: [{
                        name: 'Số giảng viên nam',
                        data: lecturerNam,
                        stack: '1'
                    },
                    {
                        name: 'Số giang viên nữ',
                        data: lecturerNu,
                        stack: '1'
                    }]
                });

            }
            else {
                showToast("lỗi thống kê", "error");
            }
        },
        error: function (xhr) {
            UnBlockUI();
            showToast("lỗi tải dữ liệu", "error");
        }
    });
}

loadReport2 = function () {
    BlockUI();
    $.ajax({
        url: "/Admin/DashBoard/Report2",
        type: "GET",
        success: function (res) {
            UnBlockUI();
            var data = res.data;
            var faculty = data.faculty;
            var studentnam = data.studentNam;
            var studentnu = data.studentNu;
            debugger;
            if (res.success) {
                Highcharts.chart('reportsChart2', {
                    chart: {
                        type: 'column'
                    },
                    title: {
                        text: 'Số lượng sinh viên theo khoa'
                    },
                    xAxis: {
                        categories: faculty, // Danh sách tên khoa từ API
                        title: {
                            text: 'Khoa'
                        }
                    },
                    tooltip: {
                        format: '<b>{key}</b><br/>{series.name}: {y}<br/>' +
                            'Total: {point.stackTotal}'
                    },

                    plotOptions: {
                        column: {
                            stacking: 'normal'
                        }
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: 'Số lượng sinh vien'
                        }
                    },
                    series: [{
                        name: 'Số sinh viên nam',
                        data: studentnam,
                        stack : '1'
                    },
                    {
                        name: 'Số sinh viên nữ',
                        data: studentnu,
                        stack : '1'
                    }]
                });

            }
            else {
                showToast("lỗi thống kê", "error");
            }
        },
        error: function (xhr) {
            UnBlockUI();
            showToast("lỗi tải dữ liệu", "error");
        }
    });
}

loadCountStu = function (fillter) {
    $.ajax({
        url: "/Admin/DashBoard/reportCountStu",
        type: "GET",
        data: { fillter: fillter },
        success: function (res) {
            if (res.success) {
                debugger;
                var data = res.data;
                var percent = data.percentIn.toFixed(2);
                var isIncrease = percent >= 0;

                var $countStu = $("#CountStu");
                var $h6 = $countStu.find("h6").eq(0);
                var $spanPercent = $countStu.find("span").eq(0);
                var $spanLabel = $countStu.find("span").eq(1);

                $h6.text("" + data.studentCurent);
                $spanPercent
                    .text((isIncrease ? "+" : "") + percent + "%")
                    .removeClass("text-success text-danger")
                    .addClass(isIncrease ? "text-success" : "text-danger");

                $spanLabel.text(isIncrease ? "increase" : "decrease");
            } else {
                showToast("lỗi thống kê", "error");
            }
        },
        error: function () {
            showToast("lỗi tải dữ liệu", "error");
        }
    });
}
loadCountLecturer = function (fillter) {
    $.ajax({
        url: "/Admin/DashBoard/reportCountLeturer",
        type: "GET",
        data: { fillter: fillter },
        success: function (res) {
            if (res.success) {
                debugger;
                var data = res.data;
                var percent = data.percentIn.toFixed(2);
                var isIncrease = percent >= 0;

                var $countLecturer = $("#CountLec");
                var $h6 = $countLecturer.find("h6").eq(0);
                var $spanPercent = $countLecturer.find("span").eq(0);
                var $spanLabel = $countLecturer.find("span").eq(1);
                $h6.text("" + data.lecturerCurent);
                $spanPercent
                    .text((isIncrease ? "+" : "") + percent + "%")
                    .removeClass("text-success text-danger")
                    .addClass(isIncrease ? "text-success" : "text-danger");

                $spanLabel.text(isIncrease ? "increase" : "decrease");
            } else {
                showToast("lỗi thống kê", "error");
            }
        },
        error: function () {
            showToast("lỗi tải dữ liệu", "error");
        }
    });
}
loadCountRoom = function (fillter) {
    $.ajax({
        url: "/Admin/DashBoard/reportCountRoom",
        type: "GET",
        data: { fillter: fillter },
        success: function (res) {
            if (res.success) {
                debugger;
                var data = res.data;
                var percent = data.percentIn.toFixed(2);
                var isIncrease = percent >= 0;

                var $countRoom = $("#CountRoom");
                var $h6 = $countRoom.find("h6").eq(0);
                var $spanPercent = $countRoom.find("span").eq(0);
                var $spanLabel = $countRoom.find("span").eq(1);

                $h6.text("" + data.roomCurent);
                $spanPercent
                    .text((isIncrease ? "+" : "") + percent + "%")
                    .removeClass("text-success text-danger")
                    .addClass(isIncrease ? "text-success" : "text-danger");

                $spanLabel.text(isIncrease ? "increase" : "decrease");
            } else {
                showToast("lỗi thống kê", "error");
            }
        },
        error: function () {
            showToast("lỗi tải dữ liệu", "error");
        }
    });
}

loadCountStu();
loadCountLecturer();
loadCountRoom();
loadReport1();
loadReport2();
