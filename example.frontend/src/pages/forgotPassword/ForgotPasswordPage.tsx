import { usePageTitle } from "../../helpers/pageTitleHelper";
import { useForgotPassword } from "../../query/authQueries";
import { FormikErrors, FormikProvider, useFormik, Form } from "formik";
import { ForgotPasswordRequestDto } from "../../query/types/auth";
import { Loading } from "../../components/loading/Loading";
import { StronglyTypedField } from "../../components/formik/StronglyTypedField";
import { FormikInput } from "../../components/formik/FormikInput";
import toast from "react-hot-toast";
import { useState } from "react";
import { useSearchParams } from "react-router-dom";

type FormFields = ForgotPasswordRequestDto & {};

export function ForgotPasswordPage() {
    usePageTitle("Forgot Password");

    const [search] = useSearchParams();
    const email = search.get("email");

    const forgotPassword = useForgotPassword();

    const [isSuccess, setIsSuccess] = useState(false);

    const formik = useFormik<FormFields>({
        initialValues: {
            email: email ?? ''
        },
        enableReinitialize: true,
        validate: validate,
        onSubmit: (values) => {
            forgotPassword.mutate(values, {
                onSuccess: () => {
                    toast.success('Forgotten password email sent.');
                    setIsSuccess(true);
                },
                onError: (error) => {
                    toast.error(`Forgotten password email error. ${error.title ?? ""}`);
                }
            });
        }
    });

    return (
        <div className="col-12 col-lg-6 text-center">
            <h1>Forgot Password</h1>
            <Loading loading={forgotPassword.isPending} transparent>
                {
                    isSuccess &&
                    <p>
                        Forgotten password email sent.
                        Please check your email.
                    </p>
                }
                {
                    !isSuccess &&
                    <FormikProvider value={formik}>
                        <Form>
                            <p>Enter your email address to reset your password.</p>
                            <StronglyTypedField<FormFields>
                                name="email"
                                label="Email"
                                type="text"
                                required
                                component={FormikInput}
                                disabled={forgotPassword.isPending}
                            />
                            <button type="submit"
                                className="btn btn-primary w-100"
                                disabled={forgotPassword.isPending}
                            >
                                Forgot Password
                            </button>
                        </Form>
                    </FormikProvider>
                }
            </Loading>
        </div>
    );
}

function validate(values: FormFields): FormikErrors<FormFields> {
    const errors: FormikErrors<FormFields> = {};

    if (!values.email) {
        errors.email = "Email is required.";
    }

    return errors;
}