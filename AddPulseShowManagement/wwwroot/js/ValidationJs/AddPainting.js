$(document).ready(function () {

    $("#AddPaintingForm").validate({
        errorClass: 'Error',
        rules:
        {
            NameOfArtist:
            {
                required: true,
            },
            ImagePath:
            {
                required: true,
            },
            NameOfPainting:
            {
                required: true,
            },
            YearOfPainting:
            {
                required: true,
            }
        },
        messages:
        {
            NameOfArtist:
            {
                required: "This field is required.",
            },
            ImagePath:
            {
                required: "This field is required.",
            },
            NameOfPainting:
            {
                required: "This field is required.",
            },
            YearOfPainting:
            {
                required: "This field is required.",
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