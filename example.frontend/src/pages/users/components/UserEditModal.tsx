import { Form, FormikErrors, FormikProvider, useFormik } from "formik";
import { UserDto } from "../../../query/types/admin";
import { useUpdateUser } from "../../../query/adminQueries";
import toast from "react-hot-toast";
import { Modal } from "../../../components/layout/Modal";
import { StronglyTypedField } from "../../../components/formik/StronglyTypedField";
import { FormikInput } from "../../../components/formik/FormikInput";
import { FormikCheckbox } from "../../../components/formik/FormikCheckbox";
import { useAuth } from "../../../query/authQueries";
import { useMemo } from "react";

export function UserEditModal({
    user,
    visible,
    onClose
}: Readonly<{
    user?: UserDto,
    visible: boolean,
    onClose: () => void
}>) {
    const auth = useAuth();
    const updateUser = useUpdateUser();

    const formik = useFormik<UserDto>({
        initialValues: {
            id: user?.id ?? 0,
            email: user?.email ?? '',
            firstName: user?.firstName ?? '',
            lastName: user?.lastName ?? '',
            emailVerified: user?.emailVerified ?? false,
            isAdmin: user?.isAdmin ?? false,
            registrationDate: (user?.registrationDate ? new Date(user.registrationDate) : new Date()).toISOString().substring(0, 16),
            emailVerificationDate: user?.emailVerificationDate ? new Date(user.emailVerificationDate).toISOString().substring(0, 16) : undefined
        },
        validate: validate,
        enableReinitialize: true,
        onSubmit: values => {
            updateUser.mutate(values, {
                onSuccess: () => {
                    toast.success("User updated successfully");
                    onClose();
                },
                onError: (error) => {
                    toast.error(`Failed to update user: ${error}`);
                }
            });
        }
    });

    const handleClose = () => {
        formik.resetForm();
        onClose();
    }

    const isMe = useMemo(() => {
        return auth.user?.email === user?.email;
    }, [auth.user, user?.email]);

    if (!user) return null;

    return (
        <Modal isVisible={visible}
            reject={handleClose}
            closeOnBackdropClick={false}
            size="lg"
            title={`Edit user ${user.email}`}>
            {
                isMe &&
                <div className="alert alert-warning" role="alert">
                    You are editing your own account. Be careful!
                </div>
            }
            <FormikProvider value={formik}>
                <Form>
                    <StronglyTypedField<UserDto>
                        name="email"
                        label="Email"
                        component={FormikInput}
                        disabled
                    />
                    <StronglyTypedField<UserDto>
                        name="firstName"
                        label="First name"
                        component={FormikInput}
                    />
                    <StronglyTypedField<UserDto>
                        name="lastName"
                        label="Last name"
                        component={FormikInput}
                    />
                    <StronglyTypedField<UserDto>
                        name="emailVerified"
                        label="Email verified"
                        type="checkbox"
                        component={FormikCheckbox}
                    />
                    <StronglyTypedField<UserDto>
                        name="isAdmin"
                        label="Admin"
                        type="checkbox"
                        component={FormikCheckbox}
                    />
                    <StronglyTypedField<UserDto>
                        name="registrationDate"
                        label="Registration date"
                        type="datetime-local"
                        component={FormikInput}
                        disabled
                    />
                    <StronglyTypedField<UserDto>
                        name="emailVerificationDate"
                        label="Email verification date"
                        type="datetime-local"
                        component={FormikInput}
                        disabled
                    />

                    <div className="d-flex justify-content-between">
                        <button type="submit" className="btn btn-primary">Save</button>
                        <button type="button" className="btn btn-secondary ms-2" onClick={handleClose}>Close</button>
                    </div>
                </Form>
            </FormikProvider>
        </Modal>
    )
}

function validate(values: UserDto) {
    const errors: FormikErrors<UserDto> = {};

    if (!values.email) {
        errors.email = 'Email is required';
    }

    if (values.emailVerified !== true && values.isAdmin === true) {
        errors.isAdmin = 'Admin users must have verified email';
    }

    return errors;
}