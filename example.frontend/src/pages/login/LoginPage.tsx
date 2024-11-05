import { Link, useNavigate } from "react-router-dom";
import { usePageTitle } from "../../helpers/pageTitleHelper";
import { useLogin } from "../../query/authQueries";
import { Form, FormikErrors, FormikProvider, useFormik } from "formik";
import { AuthenticateRequestDto } from "../../query/types/auth";
import { FormikInput } from "../../components/formik/FormikInput";
import { StronglyTypedField } from "../../components/formik/StronglyTypedField";
import { Loading } from "../../components/loading/Loading";

type FormFields = AuthenticateRequestDto & {};

export function LoginPage() {
    usePageTitle("Login Page");

    const nav = useNavigate();
    const login = useLogin();

    const formik = useFormik<FormFields>({
        initialValues: {
            email: '',
            password: ''
        },
        validate: validate,
        onSubmit: (values) => {
            login.mutate(values, {
                onSuccess: () => {
                    nav('/');
                }
            });
        }
    });

    return (
        <div className="col-12 col-lg-6 text-center">
            <h1>Login</h1>
            <div style={{ minWidth: '50%' }}>
                <Loading loading={login.isPending} transparent>
                    <FormikProvider value={formik}>
                        <Form>
                            <StronglyTypedField<FormFields>
                                name="email"
                                label="Email"
                                type="text"
                                required
                                component={FormikInput}
                                disabled={login.isPending}
                            />
                            <StronglyTypedField<FormFields>
                                name="password"
                                label="Password"
                                type="password"
                                required
                                component={FormikInput}
                                disabled={login.isPending}
                            />
                            <div className="text-center">
                                <button type="submit"
                                    className="btn btn-primary w-100"
                                    disabled={login.isPending}
                                >
                                    Login
                                </button>
                                <div className="mt-2">
                                    <Link to={`/forgotpassword?email=${formik.values.email}`}>
                                        Forgot your password?
                                    </Link>
                                </div>
                            </div>
                        </Form>
                        {
                            login.isError && <div className="alert alert-danger mt-3">
                                {login.error.message}
                            </div>
                        }
                    </FormikProvider>
                </Loading>
            </div>
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

    if (values.email && !/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i.test(values.email)) {
        errors.email = 'Invalid email address';
    }

    return errors;
}
