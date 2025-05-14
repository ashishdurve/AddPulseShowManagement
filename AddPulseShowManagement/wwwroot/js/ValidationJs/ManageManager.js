
$.validator.addMethod("valueNotEquals", function (value, element, arg) {
    return arg !== value;
}, function (para, element) {
    return $.validator.format("*Please Select the Value!");
});
$(document).ready(function () {
    debugger
    //$.validator.unobtrusive.parse($('#frmAddUpdateManager'))

    $("#frmAddUpdateManager").validate({
        errorClass: 'Error',
        rules:
        {
            FirstName:
            {
                required: true,
            },
            Email:
            {
                required: true,
                email: true
            },
            Phone:
            {
                required: true,
            },
            IsActive:{
               required: false,
            }
        },
        messages:
        {
            FirstName:
            {
                required: "Please enter name.",
            },
            Email:
            {
                required: "Please enter vaild email.",
            },
            Phone:
            {
                required: "Please enter phone number.",
            }
        },
        highlight: function (element) {
            $(element).parent().addClass('error');
        },
        unhighlight: function (element) {
            $(element).parent().removeClass('error');
        },
        success: function (element) {
            debugger
            element.text('OK!').addClass(
                'valid').closest(
                    '.form-group')
                .removeClass('error')
                .addClass('success');
        },
        errorPlacement: function (error, element) {

            debugger

            if (element.hasClass('select2-hidden-accessible')) {
                error.insertAfter(element.next('span'));
                //error.css("margin-top", "30px");
            } else {
                error.insertAfter(element);
            }
        }
    });

});