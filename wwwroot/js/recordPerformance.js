// wwwroot/js/recordPerformance.js
$(document).ready(function () {
  $('.show-performance').click(function () {
    var firstName = $(this).data('first-name')
    var lastName = $(this).data('last-name')
    $('#fullNameDisplay').text(firstName + ' ' + lastName)
  })
})
// نمایش modal تصویر قرارداد - نسخه بهینه‌شده
$(document).on('click', '.show-contract', function () {
  var recordId = $(this).data('id')
  $('#contractRecordId').val(recordId)
  $('#contractImagePreview').hide()
  $('#noContractImage').show()
  $('#contractFile').val('')

  // دریافت تصویر قرارداد از سرور
  fetch(`/Record/GetContractImage?id=${recordId}`)
    .then(response => {
      if (!response.ok) {
        // اگر وضعیت 404 بود (تصویر وجود ندارد)، پیام خطا نمایش ندهید
        if (response.status === 404) {
          return { imageUrl: null }
        }
        throw new Error('خطا در دریافت تصویر')
      }
      return response.json()
    })
    .then(data => {
      if (data.imageUrl) {
        $('#contractImagePreview').attr('src', data.imageUrl).show()
        $('#noContractImage').hide()
      }
    })
    .catch(error => {
      // فقط خطاهای غیر از 404 را نمایش دهید
      if (!error.message.includes('404')) {
        console.error('Error:', error)
        toastr.error('خطا در دریافت تصویر قرارداد', 'خطا')
      }
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

// آپلود تصویر - نسخه بهبودیافته
$('#uploadContract').click(function () {
  var fileInput = $('#contractFile')[0]

  // بررسی اینکه آیا فایلی انتخاب شده است
  if (!fileInput.files || fileInput.files.length === 0) {
    toastr.error('لطفاً یک فایل تصویر انتخاب کنید', 'خطا')
    return
  }

  var formData = new FormData($('#contractForm')[0])

  fetch('/Record/UploadContract', {
    method: 'POST',
    body: formData
  })
    .then(response => {
      if (!response.ok) {
        return response.json().then(err => {
          throw new Error(err.message || 'خطا در ذخیره تصویر')
        })
      }
      return response.json()
    })
    .then(data => {
      toastr.success('تصویر با موفقیت ذخیره شد', 'موفقیت')
      $('#contractImagePreview').attr('src', data.imageUrl).show()
      $('#noContractImage').hide()
      $('#contractFile').val('') // پاک کردن فایل انتخاب شده
    })
    .catch(error => {
      console.error('Error:', error)
      toastr.error(error.message || 'خطا در ذخیره تصویر', 'خطا')
    })
})
let counter = 0
$(document).ready(function () {
  // در بخش ایجاد ردیف‌ها، این تغییرات را اعمال کنید
  $('#performanceModal').on('show.bs.modal', function (event) {
    if (counter == 0) {
      counter++
      var button = $(event.relatedTarget)
      var userId = button.data('id')
      var firstName = button.data('first-name')
      var lastName = button.data('last-name')

      $('#fullNameDisplay').text(firstName + ' ' + lastName)
      $('#performanceModal').data('user-id', userId)
      $('#performanceTableBody').empty()

      $.get('/Performance/GetPerformanceData', { userId: userId })
        .done(function (data) {
          console.log('رکوردهای دریافت شده:', data)

          // تعیین تعداد رکوردها
          var recordCount = data.length

          data.forEach(function (record, index) {
            // آیا این رکورد آخرین رکورد است؟
            var isLastRecord = index === recordCount - 1

            var row = `
                      <tr data-month="${record.month}">
                          <td>${record.monthName}</td>
                          <td>
                              <select class="form-select" name="work" ${
                                isLastRecord ? '' : 'disabled'
                              }>
                                  ${Array.from(
                                    { length: 32 },
                                    (_, i) => `
                                      <option value="${i}" ${
                                      record.work === i ? 'selected' : ''
                                    }>${i}</option>
                                  `
                                  ).join('')}
                              </select>
                          </td>
                          <td>
                              <select class="form-select" name="vacation" ${
                                isLastRecord ? '' : 'disabled'
                              }>
                                  ${Array.from(
                                    { length: 32 },
                                    (_, i) => `
                                      <option value="${i}" ${
                                      record.vacation === i ? 'selected' : ''
                                    }>${i}</option>
                                  `
                                  ).join('')}
                              </select>
                          </td>
                          <td>
                              <select class="form-select" name="vacation_sick" ${
                                isLastRecord ? '' : 'disabled'
                              }>
                                  ${Array.from(
                                    { length: 32 },
                                    (_, i) => `
                                      <option value="${i}" ${
                                      record.vacation_sick === i
                                        ? 'selected'
                                        : ''
                                    }>${i}</option>
                                  `
                                  ).join('')}
                              </select>
                          </td>
                          <td>
                              <select class="form-select" name="mission" ${
                                isLastRecord ? '' : 'disabled'
                              }>
                                  ${Array.from(
                                    { length: 32 },
                                    (_, i) => `
                                      <option value="${i}" ${
                                      record.mission === i ? 'selected' : ''
                                    }>${i}</option>
                                  `
                                  ).join('')}
                              </select>
                          </td>
                          <td>
                              <input type="number" class="form-control" name="overtime_system" min="0" 
                                  value="${record.overtime_system || 0}" ${
              isLastRecord ? '' : 'disabled'
            }>
                          </td>
                          <td>
                              <input type="number" class="form-control" name="overtime_final" min="0" 
                                  value="${record.overtime_final || 0}" ${
              isLastRecord ? '' : 'disabled'
            }>
                          </td>
                          <td>
                              <span class="form-control-plaintext">${
                                record.sum_work || 0
                              } روز</span>
                          </td>
                      </tr>
                  `
            $('#performanceTableBody').append(row)
          })
        })
        .fail(function (error) {
          console.error('خطا در دریافت داده:', error)
        })
    } else {
      counter = 0
    }
  })
  $('#performanceModal .btn-primary').on('click', function () {
    var userId = $('#performanceModal').data('user-id')
    var records = []
    const persianMonthsMap = {
      فروردین: 0,
      اردیبهشت: 1,
      خرداد: 2,
      تیر: 3,
      مرداد: 4,
      شهریور: 5,
      مهر: 6,
      آبان: 7,
      آذر: 8,
      دی: 9,
      بهمن: 10,
      اسفند: 11
    }

    // فقط آخرین ردیف را پردازش کنید
    var lastRow = $('#performanceTableBody tr').last()
    var monthName = lastRow.find('td:first-child').text().trim()
    var month = persianMonthsMap[monthName]

    if (month === undefined) {
      console.error(`نام ماه "${monthName}" نامعتبر است.`)
      return
    }

    records.push({
      user_id: userId,
      month: month,
      work: parseInt(lastRow.find('select[name="work"]').val()),
      vacation: parseInt(lastRow.find('select[name="vacation"]').val()),
      vacation_sick: parseInt(
        lastRow.find('select[name="vacation_sick"]').val()
      ),
      mission: parseInt(lastRow.find('select[name="mission"]').val()),
      overtime_system: parseInt(
        lastRow.find('input[name="overtime_system"]').val()
      ),
      overtime_final: parseInt(
        lastRow.find('input[name="overtime_final"]').val()
      )
    })

    $.ajax({
      url: '/Performance/SavePerformanceData',
      type: 'POST',
      contentType: 'application/json',
      data: JSON.stringify(records),
      success: function (response) {
        if (response.success) {
          toastr.success('تغییرات با موفقیت ذخیره شد', 'موفقیت')
        } else {
          toastr.error('خطا در ذخیره تغییرات', 'خطا')
        }
      },
      error: function () {
        toastr.error('خطا در ارسال داده‌ها به سرور', 'خطا')
      }
    })
  })
})
// مدیریت دیالوگ تأیید حذف
$(document).on('click', '.delete-btn', function () {
  var recordId = $(this).data('id')
  $('#deleteForm input[name="id"]').val(recordId)
})
