// wwwroot/js/recordPerformance.js
$(document).ready(function () {
  $('.show-performance').click(function () {
    var firstName = $(this).data('first-name')
    var lastName = $(this).data('last-name')
    $('#fullNameDisplay').text(firstName + ' ' + lastName)
    // اعتبارسنجی شماره شبا
    $('input[placeholder="IR..."]').on('input', function () {
      var sheba = $(this).val()
      if (sheba.length === 24 && sheba.startsWith('IR')) {
        $(this).removeClass('is-invalid').addClass('is-valid')
      } else {
        $(this).removeClass('is-valid').addClass('is-invalid')
      }
    })
  })
})
// نمایش modal تصویر قرارداد
$(document).on('click', '.show-contract', function () {
  var recordId = $(this).data('id')
  $('#contractRecordId').val(recordId)

  // دریافت تصویر قرارداد از سرور
  $.get('/Record/GetContractImage?id=' + recordId, function (data) {
    if (data.imageUrl) {
      $('#contractImagePreview').attr('src', data.imageUrl).show()
      $('#noContractImage').hide()
    } else {
      $('#contractImagePreview').hide()
      $('#noContractImage').show()
    }
  }).fail(function () {
    // alert('خطا در دریافت تصویر قرارداد.');
  })
})

// پیش‌نمایش تصویر قبل از آپلود
$('#contractFile').change(function () {
  var file = this.files[0]
  if (file) {
    var reader = new FileReader()
    reader.onload = function (e) {
      $('#contractImagePreview').attr('src', e.target.result).show()
      $('#noContractImage').hide()
    }
    reader.readAsDataURL(file)
  }
})

// آپلود تصویر
$('#uploadContract').click(function () {
  var formData = new FormData($('#contractForm')[0])
  $.ajax({
    url: '/Record/UploadContract',
    type: 'POST',
    data: formData,
    processData: false,
    contentType: false,
    success: function (response) {
      alert('تصویر با موفقیت ذخیره شد')
    },
    error: function () {
      alert('خطا در ذخیره تصویر')
    }
  })
})
$(document).ready(function () {
  // هنگام نمایش مودال
  $('#performanceModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget)
    var userId = button.data('id')

    // دریافت داده‌ها از سرور
    $.get('/Performance/GetPerformanceData', { userId: userId })
      .done(function (data) {
        console.log('رکوردهای دریافت شده:', data)

        // نمایش اطلاعات در کنسول به صورت جزئی‌تر
        data.forEach(function (record) {
          console.log(
            `ماه: ${record.month} | ` +
              `کارکرد: ${record.work} روز | ` +
              `مرخصی استحقاقی: ${record.vacation} روز`
          )
        })
      })
      .fail(function (error) {
        console.error('خطا در دریافت داده:', error)
      })
  })
})
// مدیریت دیالوگ تأیید حذف
$(document).on('click', '.delete-btn', function () {
  var recordId = $(this).data('id')
  $('#deleteForm input[name="id"]').val(recordId)
})
