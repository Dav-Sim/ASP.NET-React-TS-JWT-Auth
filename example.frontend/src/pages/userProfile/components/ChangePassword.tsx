import { FormikProvider, useFormik, Form } from "formik";
import { useAuth, useChangePassword } from "../../../query/authQueries";
import { ChangePasswordRequestDto } from "../../../query/types/auth";
import toast from "react-hot-toast";
import { StronglyTypedField } from "../../../components/formik/StronglyTypedField";
import { FormikInput } from "../../../components/formik/FormikInput";

type FormFields = ChangePasswordRequestDto & {};

export function ChangePassword() {
    const changePassword = useChangePassword();
    const auth = useAuth();

    const formik = useFormik<FormFields>({
        initialValues: {
            password: '',
            newPassword: '',
        },
        validate: validate,
        onSubmit: async (values) => {
            await changePassword.mutateAsync(values, {
                onSuccess: () => {
                    //clear the form
                    formik.resetForm();
                    toast.success('Password changed successfully');
                },
                onError: (error) => {
                    toast.error(`Error: ${error.message}`);
                }
            });
        }
    });

    return (
        <div>
            <h3>Change password</h3>

            <FormikProvider value={formik}>
                <Form>
                    <input type="hidden" name="email" value={auth.user?.email ?? ""} />
                    <div className="mb-3">
                        <StronglyTypedField<FormFields>
                            name="password"
                            type="password"
                            required
                            label="Current Password"
                            component={FormikInput} />
                    </div>
                    <div className="mb-3">
                        <StronglyTypedField<FormFields>
                            name="newPassword"
                            type="password"
                            required
                            label="New Password"
                            component={FormikInput} />
                    </div>
                    <button type="submit"
                        className="btn btn-primary w-100">
                        Change Password
                    </button>
                </Form>
            </FormikProvider>
        </div>
    );
}

function validate(values: FormFields) {
    const errors: Partial<FormFields> = {};

    if (!values.password) {
        errors.password = 'Required';
    }

    if (!values.newPassword) {
        errors.newPassword = 'Required';
    }

    return errors;
}
