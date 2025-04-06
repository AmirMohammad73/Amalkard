// wwwroot/js/recordPerformance.js
$(document).ready(function() {
    $('.show-performance').click(function() {
        var firstName = $(this).data('first-name');
        var lastName = $(this).data('last-name');
        $('#fullNameDisplay').text(firstName + ' ' + lastName);
        // اعتبارسنجی شماره شبا
        $('input[placeholder="IR..."]').on('input', function() {
            var sheba = $(this).val();
            if(sheba.length === 24 && sheba.startsWith('IR')) {
                $(this).removeClass('is-invalid').addClass('is-valid');
            } else {
                $(this).removeClass('is-valid').addClass('is-invalid');
            }
        });
    });
});
// نمایش modal تصویر قرارداد
$(document).on('click', '.show-contract', function() {
    var recordId = $(this).data('id');
    $('#contractRecordId').val(recordId);
    
    // در اینجا می‌توانید تصویر موجود را از سرور دریافت و نمایش دهید
    // مثال:
    /*
    $.get('/Records/GetContractImage?id=' + recordId, function(data) {
        if(data.imageUrl) {
            $('#contractImagePreview').attr('src', data.imageUrl).show();
            $('#noContractImage').hide();
        } else {
            $('#contractImagePreview').hide();
            $('#noContractImage').show();
        }
    });
    */
});

// پیش‌نمایش تصویر قبل از آپلود
$('#contractFile').change(function() {
    var file = this.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function(e) {
            $('#contractImagePreview').attr('src', e.target.result).show();
            $('#noContractImage').hide();
        }
        reader.readAsDataURL(file);
    }
});

// آپلود تصویر
$('#uploadContract').click(function() {
    var formData = new FormData($('#contractForm')[0]);
    
    $.ajax({
        url: '/Records/UploadContract',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function(response) {
            alert('تصویر با موفقیت ذخیره شد');
            $('#contractModal').modal('hide');
        },
        error: function() {
            alert('خطا در ذخیره تصویر');
        }
    });
});
// مدیریت دیالوگ تأیید حذف
$(document).on('click', '.delete-btn', function() {
    var recordId = $(this).data('id');
    $('#deleteForm input[name="id"]').val(recordId);
});