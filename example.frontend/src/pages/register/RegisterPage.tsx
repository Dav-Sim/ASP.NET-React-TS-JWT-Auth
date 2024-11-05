import { useNavigate } from "react-router-dom";
import { usePageTitle } from "../../helpers/pageTitleHelper";
import { RegisterRequestDto } from "../../query/types/auth";
import { useRegister } from "../../query/authQueries";
import { Form, FormikErrors, FormikProvider, useFormik } from "formik";
import { Loading } from "../../components/loading/Loading";
import { StronglyTypedField } from "../../components/formik/StronglyTypedField";
import { FormikInput } from "../../components/formik/FormikInput";
import toast from "react-hot-toast";

type FormFields = RegisterRequestDto & {};

export function RegisterPage() {
    usePageTitle("Register Page");

    const nav = useNavigate();
    const register = useRegister();

    const formik = useFormik<FormFields>({
        initialValues: {
            email: '',
            password: ''
        },
        validate: validate,
        onSubmit: (values) => {
            register.mutate(values, {
                onSuccess: () => {
                    toast.success('Registration successful.');
                    nav('/');
                },
                onError: (error) => {
                    toast.error(`Registration failed. ${error.title ?? ""}`);
                }
            });
        }
    });


    return (
        <div className="col-12 col-lg-6 text-center">
            <h1>Register</h1>
            <Loading loading={register.isPending} transparent>
                <FormikProvider value={formik}>
                    <Form>
                        <StronglyTypedField<FormFields>
                            name="email"
                            label="Email"
                            type="text"
                            component={FormikInput}
                            required
                            disabled={register.isPending}
                        />

                        <StronglyTypedField<FormFields>
                            name="firstName"
                            label="First Name"
                            type="text"
                            component={FormikInput}
                            disabled={register.isPending}
                        />

                        <StronglyTypedField<FormFields>
                            name="lastName"
                            label="Last Name"
                            type="text"
                            component={FormikInput}
                            disabled={register.isPending}
                        />

                        <StronglyTypedField<FormFields>
                            name="password"
                            label="Password"
                            type="password"
                            required
                            component={FormikInput}
                            disabled={register.isPending}
                        />
                        <button type="submit"
                            className="btn btn-primary w-100"
                            disabled={register.isPending}
                        >
                            Register
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
        errors.email = 'Email is required';
    }

    if (!values.password) {
        errors.password = 'Password is required';
    }

    if (values.email && !/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i.test(values.email)) {
        errors.email = 'Invalid email address';
    }

    return errors;
}