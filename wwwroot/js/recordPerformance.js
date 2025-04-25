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
$('#exportButton').click(function () {
  var selectedMonth = $('#monthDropdown').val()
  window.location.href = `/Record/ExportToExcel?month=${selectedMonth}`
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

          var recordCount = data.length

          data.forEach(function (record, index) {
            console.log("record.canEditOvertimeFinal:", record.canEditOvertimeFinal);
            var isLastRecord = index === recordCount - 1
            var canEditOvertimeFinal =
              record.canEditOvertimeFinal && isLastRecord
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
                                          record.vacation === i
                                            ? 'selected'
                                            : ''
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
              canEditOvertimeFinal ? '' : 'disabled'
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
})
// مدیریت دیالوگ تأیید حذف
$(document).on('click', '.delete-btn', function () {
  var recordId = $(this).data('id')
  $('#deleteForm input[name="id"]').val(recordId)
})
