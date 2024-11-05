import { FieldProps, getIn } from "formik";
import { useMemo } from "react";

export function FormikCheckbox({
    field,
    form,
    label,
    ...props
}: Readonly<
    FieldProps & {
        label: string
    }>) {
    const { error, touched } = useMemo(() => {
        return {
            error: getIn(form.errors, field.name),
            touched: getIn(form.touched, field.name)
        };
    }, [field.name, form.errors, form.touched]);

    const isError = useMemo(() => {
        return touched && !!error;
    }, [touched, error]);

    return (
        <div className="mb-3 text-start form-check">
            <input id={field.name} {...props} {...field} className={`form-check-input ${isError ? 'is-invalid' : ''}`} />
            <label htmlFor={field.name} className="form-check-label">{label}</label>
            {isError && <div className="invalid-feedback">{error}</div>}
        </div>
    );
}