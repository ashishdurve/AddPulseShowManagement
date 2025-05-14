$(document).ready(function () {

    ExitGameEditorModalData();
    BackGameEditorModalData();
});

function ExitGameEditorModalData() {
    $.ajax({
        type: "GET",
        url: "/Cracked/_ExitGameEditorModal",
        success: function (data) {
            $("#ExitGameEditorModalData").html('');
            $("#ExitGameEditorModalData").html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            DisplayMessage(ErrorCross + SpanStart + "Something Went Wrong!" + SpanEnd);
        }
    });
}

function BackGameEditorModalData() {
    $.ajax({
        type: "GET",
        url: "/Cracked/_BackGameEditorModal",
        success: function (data) {
            $("#BackGameEditorModalData").html('');
            $("#BackGameEditorModalData").html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            DisplayMessage(ErrorCross + SpanStart + "Something Went Wrong!" + SpanEnd);
        }
    });
}

function CloseGameEditor(id) {
    window.location.href = "/MuseumUser/MyGameOne?GameID="+id;
}