// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// JavaScript code

$(".add-endpoint").click(
    function (event) {
        event.preventDefault();
        var endpointField = $(".endpoint-prototype").clone().removeClass("endpoint-prototype");
        endpointField.insertBefore($(".endpoint-prototype"));
    }
);