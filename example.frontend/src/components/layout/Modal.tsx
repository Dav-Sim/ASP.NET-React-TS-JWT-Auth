import React, { useMemo, useRef } from 'react';
import './Modal.css';

export function Modal({
    children,
    title,
    isVisible,
    reject,
    size,
    closeOnBackdropClick,
    stickyHeader,
    overrideZIndex

}: Readonly<{
    children: React.ReactNode,
    title: string,
    isVisible: boolean,
    reject: () => void,
    size?: 'sm' | 'md' | 'lg' | 'xl' | 'enormous',
    closeOnBackdropClick?: boolean,
    stickyHeader?: boolean,
    overrideZIndex?: number
}>) {
    const containerRef = useRef<HTMLDivElement>(null);

    const cssSize = useMemo(() => {
        switch (size) {
            case 'sm': return 'modal-sm';
            case 'md': return '';
            case 'lg': return 'modal-lg';
            case 'xl': return 'modal-xl';
            case 'enormous': return 'modal-enormous';
            default: return '';
        }
    }, [size]);

    const handleClick = (ev: React.SyntheticEvent<HTMLDivElement>) => {
        if (closeOnBackdropClick === true) {
            const modal = ev.currentTarget.getAttribute("data-modal");
            if (!modal || modal !== "outer") {
                ev.stopPropagation();
            }
            if (modal && modal === "outer") {
                reject?.();
            }
        }
    }

    const handleUpClick = (ev: React.SyntheticEvent<HTMLButtonElement>) => {
        ev.preventDefault();
        ev.currentTarget.blur();
        containerRef?.current?.scrollIntoView({ behavior: 'smooth', block: "start" });
    }

    if (!isVisible) return null;

    return (
        <div className={`modal ${cssSize} fade show`}
            data-modal="outer"
            onClick={handleClick} tabIndex={-1}
            style={{ display: 'block', backgroundColor: '#000000ab', zIndex: overrideZIndex }}
            role='presentation'>

            <div className="modal-dialog" data-modal="inner" onClick={handleClick} ref={containerRef} role='presentation'>
                <div className="modal-content position-relative">

                    <div className="modal-header bg-light text-dark shadow-sm justify-content-between"
                        style={stickyHeader ? { zIndex: '1', position: 'sticky', top: '0' } : undefined}>
                        <h5 className="modal-title">{title}</h5>
                        <div>
                            {
                                stickyHeader === true &&
                                <button type="button" className="btn btn-sm me-3" aria-label="Close" onClick={handleUpClick}>
                                    <i className="fa-solid fa-angles-up"></i>
                                </button>
                            }
                            <button type="button" className="btn btn-sm " aria-label="Close" onClick={() => reject?.()}>
                                <i className="fa-solid fa-xmark"></i>
                            </button>
                        </div>
                    </div>

                    <div className="modal-body">
                        {children}
                    </div>
                </div>
            </div>
        </div >
    );
}
