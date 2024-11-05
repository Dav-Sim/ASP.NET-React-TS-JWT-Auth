import { FieldProps, getIn } from "formik";
import { useMemo } from "react";
import './FormikInput.css';

export function FormikInput({
    field,
    form,
    label,
    required,
    ...props
}: Readonly<
    FieldProps & {
        label: string,
        required?: boolean
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
        <div className={`mb-3 text-start ${required ? "form-group-required" : ""}`}>
            <label className="form-label" htmlFor={field.name}>{label}</label>
            <input id={field.name} {...props} {...field} className={`form-control ${isError ? 'is-invalid' : ''}`} />
            {isError && <div className="invalid-feedback">{error}</div>}
        </div>
    );
}