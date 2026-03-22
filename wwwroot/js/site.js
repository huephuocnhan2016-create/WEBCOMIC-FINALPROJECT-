$("#search-input").on("keyup", function () {
    let keyword = $(this).val();
    if (keyword.length > 2) {
        $.get("/Home/SearchApi?term=" + keyword, function (data) {
            // Render kết quả gợi ý ngay dưới thanh search
            $("#search-results").html(data).show();
        });
    }
});