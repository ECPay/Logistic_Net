$(function () {
    if (controllerName === "Content" && actionName !== undefined) {
        $("#" + actionName).addClass("active");
    }

    $("#gwLoading").hide();

    $("#gwSubmit").on("click", function () {
        $("#taResponse").val("");
        $("#showResponse").html("");
        $("#gwLoading").show();
    })
})

//Ajax.BeginForm
function Complete(data) {
    $("#taResponse").val(data.responseText);    //textarea
    $("#showResponse").html(data.responseText);
    $("#gwLoading").hide();
}