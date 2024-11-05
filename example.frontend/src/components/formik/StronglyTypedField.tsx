import { Field } from "formik";

export function StronglyTypedField<T>({
    ...props
}: Readonly<{
    name: keyof T,
    label: string,
    component: unknown
} & React.InputHTMLAttributes<HTMLInputElement>>) {
    return (
        <Field {...props} />
    );
}