$(function () {
    var id = $(".row").data('image-id');
    setInterval(function () {
        $.get('/home/getcount', { id: id }, function (result) {
            $("#viewCount").text(result.viewCount);
        });
        $.get('/home/getlikes', { imageId: id }, function(result) {
            $("#likesCount").text(result.likes);
        });
    }, 500);

    $("#like-button").on('click', function () {
        $.post("/home/likeimage", { imageId: id }, function () {
            $("#like-button").html('<span class="glyphicon glyphicon-thumbs-up"></span> Liked!');
            $("#like-button").prop("disabled", true);
        });
    });
});