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

  // دریافت تصویر قرارداد از سرور با استفاده از fetch
  fetch(`/Record/GetContractImage?id=${recordId}`)
    .then(response => {
      if (!response.ok) throw new Error('خطا در دریافت تصویر')
      return response.json()
    })
    .then(data => {
      if (data.hasImage && data.imageUrl) {
        $('#contractImagePreview').attr('src', data.imageUrl).show()
        $('#noContractImage').hide()
      }
    })
    .catch(error => {
      console.error('Error:', error)
      toastr.error('خطا در دریافت تصویر قرارداد', 'خطا')
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
  var formData = new FormData($('#contractForm')[0])

  fetch('/Record/UploadContract', {
    method: 'POST',
    body: formData
  })
    .then(response => {
      if (!response.ok) {
        return response.json().then(err => {
          throw err
        })
      }
      return response.json()
    })
    .then(data => {
      toastr.success('تصویر با موفقیت ذخیره شد', 'موفقیت')
      $('#contractImagePreview').attr('src', data.imageUrl).show()
      $('#noContractImage').hide()
    })
    .catch(error => {
      console.error('Error:', error)
      toastr.error(error.message || 'خطا در ذخیره تصویر', 'خطا')
    })
})
let counter = 0
$(document).ready(function () {
  // هنگام نمایش مودال
  $('#performanceModal').on('show.bs.modal', function (event) {
    if (counter == 0) {
      counter++
      var button = $(event.relatedTarget)
      var userId = button.data('id')
      var firstName = button.data('first-name')
      var lastName = button.data('last-name')

      // نمایش نام و نام خانوادگی در مودال
      $('#fullNameDisplay').text(firstName + ' ' + lastName)
      $('#performanceModal').data('user-id', userId) // ذخیره شناسه کاربر

      // خالی کردن جدول قبل از پر کردن
      $('#performanceTableBody').empty()

      // دریافت داده‌ها از سرور
      $.get('/Performance/GetPerformanceData', { userId: userId })
        .done(function (data) {
          console.log('رکوردهای دریافت شده:', data)

          // ایجاد ردیف‌ها برای هر رکورد
          data.forEach(function (record) {
            var row = `
                      <tr data-month="${record.month}">
                          <td>${record.monthName}</td>
                          <td>
                              <select class="form-select" name="work">
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
                              <select class="form-select" name="vacation">
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
                              <select class="form-select" name="vacation_sick">
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
                              <select class="form-select" name="mission">
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
                              <input type="number" class="form-control" name="overtime_system" min="0" value="${
                                record.overtime_system || 0
                              }">
                          </td>
                          <td>
                              <input type="number" class="form-control" name="overtime_final" min="0" value="${
                                record.overtime_final || 0
                              }">
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
    var userId = $('#performanceModal').data('user-id') // شناسه کاربر
    var records = []
    // مپ ماه‌ها به اعداد
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

    // خواندن داده‌های جدول
    $('#performanceTableBody tr').each(function () {
      var monthName = $(this).find('td:first-child').text().trim() // نام ماه از ستون اول
      var month = persianMonthsMap[monthName] // تبدیل نام ماه به عدد

      if (month === undefined) {
        console.error(`نام ماه "${monthName}" نامعتبر است.`)
        return // رد کردن این ردیف
      }

      var work = $(this).find('select[name="work"]').val() // کارکرد
      var vacation = $(this).find('select[name="vacation"]').val() // مرخصی استحقاقی
      var vacation_sick = $(this).find('select[name="vacation_sick"]').val() // مرخصی استعلاجی
      var mission = $(this).find('select[name="mission"]').val() // ماموریت
      var overtime_system = $(this).find('input[name="overtime_system"]').val() // اضافه کار سیستمی
      var overtime_final = $(this).find('input[name="overtime_final"]').val() // اضافه کار نهایی

      records.push({
        user_id: userId,
        month: month, // مقدار عددی ماه
        work: work ? parseInt(work) : null,
        vacation: vacation ? parseInt(vacation) : null,
        vacation_sick: vacation_sick ? parseInt(vacation_sick) : null,
        mission: mission ? parseInt(mission) : null,
        overtime_system: overtime_system ? parseInt(overtime_system) : null,
        overtime_final: overtime_final ? parseInt(overtime_final) : null
      })
    })

    // نمایش داده‌ها در کنسول برای تست
    console.log('ماه:', records.month)

    // ارسال داده‌ها به سرور
    $.ajax({
      url: '/Performance/SavePerformanceData',
      type: 'POST',
      contentType: 'application/json',
      data: JSON.stringify(records),
      success: function (response) {
        if (response.success) {
          alert('تغییرات با موفقیت ذخیره شد.')
        } else {
          alert('خطا در ذخیره تغییرات.')
        }
      },
      error: function () {
        alert('خطا در ارسال داده‌ها به سرور.')
      }
    })
  })
})
// مدیریت دیالوگ تأیید حذف
$(document).on('click', '.delete-btn', function () {
  var recordId = $(this).data('id')
  $('#deleteForm input[name="id"]').val(recordId)
})
