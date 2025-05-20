successAction = function (res) {
    debugger;
    if (res.success) {
        UnBlockUI();
        showToast(res.message);
        if (res.url != null && res.url != "") {
            window.location.href = res.url;
        }
    }
    else {
        UnBlockUI();
        showToast(res.message);
    }
}
