import './Loading.css';

export function Loading({
    children,
    className,
    hideChilrenWhenLoading,
    loading,
    style,
    size,
    transparent,
}: Readonly<{
    children?: React.ReactNode,
    loading?: boolean,
    className?: string,
    style?: React.CSSProperties,
    size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'xxl' | 'xxxl',
    hideChilrenWhenLoading?: boolean,
    transparent?: boolean,
}>) {
    const sizeRem = {
        xs: '.8rem',
        sm: '1rem',
        md: '2rem',
        lg: '3rem',
        xl: '4rem',
        xxl: '6rem',
        xxxl: '8rem',
    }[size ?? 'md'];

    return (
        <div className={`spinner-container ${className ?? ""}`} style={style}>
            {
                loading !== false &&
                <div data-testid="loader"
                    className={`text-center spinner-overlay ${(transparent !== true ? 'spinner-overlay-gray' : '')}`}
                    style={{ padding: sizeRem }}>
                    <div className="spinner-border text-muted" role="status" style={{ width: sizeRem, height: sizeRem }}>
                        <span className="visually-hidden">Loading...</span>
                    </div>
                </div>
            }
            {
                (!hideChilrenWhenLoading || (hideChilrenWhenLoading && !loading)) &&
                children
            }
        </div>
    );
}
