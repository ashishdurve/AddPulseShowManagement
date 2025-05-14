
$(document).ready(function () {

    $("#AddUserForm").validate({
        errorClass: 'Error',
        rules:
            {
            Email:
                {
                    required: true,
                },
            Password:
                {
                    required: true,
            },
            ConfirmPassword:
            {
                required: true,
                equalTo: "#password-fieldPass"
            },
            FirstName:
                {
                    required: true,
                }
            },
        messages:
        {
            Email:
            {
                required: "Email is required!",
            },
            Password:
            {
                required: "Password is required!",
            },
            ConfirmPassword:
            {
                required: "Confirm password is required!",
                equalTo: "Password not matched!"
            },
            FirstName:
            {
                required: "FirstName is required!",
            },
            
        },
        highlight: function (element) {
            $(element).parent().addClass('error');
        },
        unhighlight: function (element) {
            $(element).parent().removeClass('error');
        },
    });

});