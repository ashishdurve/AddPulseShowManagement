
$(document).ready(function () {
   
    $("#frmLogin").validate({
        errorClass: 'Error',
        rules: {
                userEmail: {
                required: true,
                email: true
                },
                userPassword: {
                     required: true                    
                },
                chkRememberMe: {
                    required: false
                }
        },
        highlight: function (element) {
            $(element).parent().addClass('error');
        },
        unhighlight: function (element) {
            $(element).parent().removeClass('error');
        },
    });

});