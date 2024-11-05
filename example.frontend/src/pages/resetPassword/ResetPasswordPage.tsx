import { useNavigate, useSearchParams } from "react-router-dom";
import { usePageTitle } from "../../helpers/pageTitleHelper";
import { useResetPassword } from "../../query/authQueries";
import { Form, FormikErrors, FormikProvider, useFormik } from "formik";
import { ResetPasswordRequestDto } from "../../query/types/auth";
import toast from "react-hot-toast";
import { Loading } from "../../components/loading/Loading";
import { StronglyTypedField } from "../../components/formik/StronglyTypedField";
import { FormikInput } from "../../components/formik/FormikInput";

type FormFields = ResetPasswordRequestDto & {};

export function ResetPasswordPage() {
    usePageTitle("Reset Password");

    const [search] = useSearchParams();
    const email = search.get("email");
    const token = search.get("token");

    const resetPassword = useResetPassword();
    const nav = useNavigate();

    const formik = useFormik<FormFields>({
        initialValues: {
            email: email ?? '',
            password: '',
            token: token ?? ''
        },
        enableReinitialize: true,
        validate: validate,
        onSubmit: (values) => {
            resetPassword.mutate(values, {
                onSuccess: () => {
                    toast.success('Password reset successful.');
                    nav('/');
                },
                onError: (error) => {
                    toast.error(`Password reset error. ${error.title ?? ""}`);
                }
            });
        }
    });

    if (!email || !token) {
        return (
            <div className="col-12 col-lg-6 text-center">
                <h1>Reset Password</h1>
                <p>
                    Invalid reset password link.
                </p>
            </div>
        );
    }

    return (
        <div className="col-12 col-lg-6 text-center">
            <h1>Reset Password</h1>
            <Loading loading={resetPassword.isPending} transparent>
                <FormikProvider value={formik}>
                    <Form>
                        <p>
                            Please enter your new password.
                        </p>
                        <StronglyTypedField<FormFields>
                            name="email"
                            label="Email"
                            type="text"
                            required
                            component={FormikInput}
                            disabled
                        />
                        <StronglyTypedField<FormFields>
                            name="password"
                            label="Password"
                            type="password"
                            required
                            component={FormikInput}
                            disabled={resetPassword.isPending}
                        />
                        <button type="submit"
                            className="btn btn-primary w-100"
                            disabled={resetPassword.isPending}
                        >
                            Reset Password
                        </button>
                    </Form>
                </FormikProvider>
            </Loading>
        </div>
    );
}

function validate(values: FormFields) {
    const errors: FormikErrors<FormFields> = {};

    if (!values.email) {
        errors.email = 'Required';
    }

    if (!values.password) {
        errors.password = 'Required';
    }

    return errors;
}