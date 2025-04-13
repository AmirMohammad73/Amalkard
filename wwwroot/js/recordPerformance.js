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
  $('#performanceModal').one('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var userId = button.data('id');
    var firstName = button.data('first-name');
    var lastName = button.data('last-name');

    // نمایش نام و نام خانوادگی در مودال
    $('#fullNameDisplay').text(firstName + ' ' + lastName);
    $('#performanceModal').data('user-id', userId); // ذخیره شناسه کاربر

    // خالی کردن جدول قبل از پر کردن
    $('#performanceTableBody').empty();

    // دریافت داده‌ها از سرور
    $.get('/Performance/GetPerformanceData', { userId: userId })
      .done(function (data) {
        console.log("رکوردهای دریافت شده:", data);

        // ایجاد ردیف‌ها برای هر رکورد
        data.forEach(function (record) {
          var row = `
                    <tr data-month="${record.month}">
                        <td>${record.monthName}</td>
                        <td>
                            <select class="form-select" name="work">
                                ${Array.from({ length: 32 }, (_, i) => `
                                    <option value="${i}" ${record.work === i ? 'selected' : ''}>${i}</option>
                                `).join('')}
                            </select>
                        </td>
                        <td>
                            <select class="form-select" name="vacation">
                                ${Array.from({ length: 32 }, (_, i) => `
                                    <option value="${i}" ${record.vacation === i ? 'selected' : ''}>${i}</option>
                                `).join('')}
                            </select>
                        </td>
                        <td>
                            <select class="form-select" name="vacation_sick">
                                ${Array.from({ length: 32 }, (_, i) => `
                                    <option value="${i}" ${record.vacation_sick === i ? 'selected' : ''}>${i}</option>
                                `).join('')}
                            </select>
                        </td>
                        <td>
                            <select class="form-select" name="mission">
                                ${Array.from({ length: 32 }, (_, i) => `
                                    <option value="${i}" ${record.mission === i ? 'selected' : ''}>${i}</option>
                                `).join('')}
                            </select>
                        </td>
                        <td>
                            <input type="number" class="form-control" name="overtime_system" min="0" value="${record.overtime_system || 0}">
                        </td>
                        <td>
                            <input type="number" class="form-control" name="overtime_final" min="0" value="${record.overtime_final || 0}">
                        </td>
                        <td>
                            <span class="form-control-plaintext">${record.sum_work || 0} روز</span>
                        </td>
                    </tr>
                `;
          $('#performanceTableBody').append(row);
        });
      })
      .fail(function (error) {
        console.error("خطا در دریافت داده:", error);
      });
  });
  $('#performanceModal .btn-primary').on('click', function () {
    var userId = $('#performanceModal').data('user-id'); // شناسه کاربر
    var records = [];

    // خواندن داده‌های جدول
    $('#performanceTableBody tr').each(function () {
      var month = $(this).data('month'); // ماه
      var work = $(this).find('select[name="work"]').val(); // کارکرد
      var vacation = $(this).find('select[name="vacation"]').val(); // مرخصی استحقاقی
      var vacation_sick = $(this).find('select[name="vacation_sick"]').val(); // مرخصی استعلاجی
      var mission = $(this).find('select[name="mission"]').val(); // ماموریت
      var overtime_system = $(this).find('input[name="overtime_system"]').val(); // اضافه کار سیستمی
      var overtime_final = $(this).find('input[name="overtime_final"]').val(); // اضافه کار نهایی

      records.push({
        user_id: userId,
        month: month,
        work: work,
        vacation: vacation,
        vacation_sick: vacation_sick,
        mission: mission,
        overtime_system: overtime_system,
        overtime_final: overtime_final,
      });
    });
    // نمایش داده‌ها در کنسول
    console.log("داده‌های جمع‌آوری شده برای ارسال:", records);
    // ارسال داده‌ها به سرور
    $.ajax({
      url: '/Performance/SavePerformanceData',
      type: 'POST',
      contentType: 'application/json',
      data: JSON.stringify(records),
      success: function (response) {
        if (response.success) {
          alert('تغییرات با موفقیت ذخیره شد.');
        } else {
          alert('خطا در ذخیره تغییرات!');
        }
      },
      error: function () {
        alert('خطا در ارتباط با سرور!');
      },
    });
  });
});
// مدیریت دیالوگ تأیید حذف
$(document).on('click', '.delete-btn', function () {
  var recordId = $(this).data('id')
  $('#deleteForm input[name="id"]').val(recordId)
})
