import { ReactNode } from "react";

type Props = {
  children: ReactNode;
};

const VerifyEmailLayout = ({ children }: Props) => {
  return (
    <div className="min-h-screen bg-linear-to-br from-black/70 to-black flex items-center justify-center p-4">
      <div className="container">{children}</div>
    </div>
  );
};

export default VerifyEmailLayout;
