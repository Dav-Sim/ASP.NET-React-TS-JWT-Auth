import Swal, { SweetAlertOptions } from "sweetalert2";

export function Question(title: string, text: string, callback?: () => void, options?: SweetAlertOptions) {
    if (window)
        Swal.fire({
            confirmButtonColor: 'var(--bs-info)', //'#0dcaf0',
            denyButtonColor: 'var(--bs-secondary)', //'#6c757d',
            toast: false,
            title: `<h6>${title}</h6>`,
            text: text,
            html: `<p class='m-0'>${text}</p>`,
            position: "center",
            confirmButtonText: "Ok",
            denyButtonText: "Cancel",
            showDenyButton: true,
            focusDeny: true,
            focusConfirm: false,
            inputAutoFocus: false,
            ...options
        }).then((res) => {
            if (res.isConfirmed) {
                callback?.();
            }
        });
}

export function QuestionWithInput(
    title: string,
    text: string,
    inputLabel: string,
    inputRequired: boolean,
    callback: (inputValue: string) => void
) {

    Swal.fire({
        title: title,
        html: text,
        position: 'top',
        showCancelButton: true,
        confirmButtonColor: 'var(--bs-primary)',
        denyButtonColor: 'var(--bs-light)',
        confirmButtonText: 'Yes',
        focusDeny: true,
        focusCancel: true,
        focusConfirm: false,
        input: "text",
        inputLabel: inputLabel,
        inputValidator: (val) => {
            if (!val && inputRequired) return "Required";
            else return "";
        }
    }).then((result) => {
        if (result.isConfirmed) {
            callback(result.value);
        }
    })
}