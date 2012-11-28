$(document).ready(function ()
{
    function initSelection(img, selection)
    {
        preview(img, selection);
        saveCoords(img, selection);
    }

    function preview(img, selection)
    {
        var imgWidth = $('#ImageWidth').val();
        var imgHeight = $('#ImageHeight').val();
        
        var wall_scaleX = 210 / (selection.width || 1);
        var wall_scaleY = 280 / (selection.height || 1);
        $('#wall_preview > img').css({
            width: Math.round(wall_scaleX * imgWidth) + 'px',
            height: Math.round(wall_scaleY * imgHeight) + 'px',
            marginLeft: '-' + Math.round(wall_scaleX * selection.x1) + 'px',
            marginTop: '-' + Math.round(wall_scaleY * selection.y1) + 'px'
        });
    }

    function saveCoords(img, selection)
    {
        $('input[name="X1"]').val(selection.x1);
        $('input[name="Y1"]').val(selection.y1);
        $('input[name="X2"]').val(selection.x2);
        $('input[name="Y2"]').val(selection.y2);
    }

    $('#crop_form_show_button .f_button').click(function ()
    {
        var imgWidth = $('#ImageWidth').val();
        var imgHeight = $('#ImageHeight').val();

        if (imgWidth > imgHeight)
        {
            var X1 = Math.round(imgWidth / 3);
            var Y1 = Math.round(imgHeight / 3);
            var Y2 = imgHeight - Y1;
            var X2 = X1 + Math.round(3 * (Y2 - Y1) / 4);
        }
        else
        {
            var X1 = Math.round(imgWidth / 3);
            var Y1 = Math.round(imgHeight / 3);
            var X2 = imgWidth - X1;
            var Y2 = Y1 + Math.round(4 * (X2 - X1) / 3);
        }

        $('#current_logo_image').imgAreaSelect({
            aspectRatio: '3:4',
            onSelectEnd: saveCoords,
            onSelectChange: preview,
            onInit: initSelection,
            imageWidth: imgWidth,
            imageHeight: imgHeight,
            x1: X1,
            y1: Y1,
            x2: X2,
            y2: Y2
        }
        );
        $('#crop_controlls').show();
        $('#crop_form_show_button').hide();
        $('#upload').hide();
    });

    $('#cancel_crop').click(function ()
    {
        $('#crop_controlls').hide();
        $('#crop_form_show_button').show();
        $('#upload').show();
        $('#current_logo_image').imgAreaSelect({
            remove: true
        }
        );
    });
});