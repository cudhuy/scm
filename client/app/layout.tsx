import type { Metadata } from 'next';
import { Lobster, Vollkorn } from 'next/font/google';
import './globals.css';
import ReactQueryProvider from '@/providers/react-query-provider';
import { Toaster } from '@/components/ui/sonner';
import { SessionProvider } from 'next-auth/react';
import Script from 'next/script';

const lobster = Lobster({
  weight: ['400'],
  variable: '--font-lobster',
  subsets: ['latin'],
  display: 'swap',
});

const vollkorn = Vollkorn({
  variable: '--font-vollkorn',
  subsets: ['latin'],
  display: 'swap',
});

export const metadata: Metadata = {
  metadataBase: new URL(process.env.NEXT_PUBLIC_BASE_URL || 'https://xomcafe.io.vn'),
  title: 'Xóm Cafe | Không gian cà phê thư giãn tại Hà Nội',
  description:
    'Xóm Cafe - Nơi thưởng thức cà phê chất lượng, đồ uống đặc sản và không gian yên tĩnh lý tưởng cho làm việc và gặp gỡ bạn bè tại Hà Nội.',
  keywords:
    'xom cafe, xom cafe hanoi, xom cafe ha noi, xom cafe tai ha noi, ca phe cau giay, cafe cau giay, cà phê Cầu Giấy, Cafe Cầu Giấy, ca phe nam tu liem, cafe nam tu liem, cà phê Nam Từ Liêm, Cafe Nam Từ Liêm, cà phê Thanh Xuân, cafe Thanh Xuân, cafe thanh xuan, ca phe thanh xuan, quan ca fe, quan ca phe, quán cà phê, cà phê Hà Nội, quán cà phê yên tĩnh, quán cà phê làm việc',
  openGraph: {
    title: 'Xóm Cafe | Không gian cà phê thư giãn tại Hà Nội',
    description:
      'Thưởng thức cà phê chất lượng và đồ uống đặc sản trong không gian yên tĩnh tại Xóm Cafe',
    images: [
      {
        url: '/logo-header-admin.png',
        width: 1200,
        height: 630,
        alt: 'Xóm Cafe',
      },
    ],
    locale: 'vi_VN',
    type: 'website',
    siteName: 'Xóm Cafe',
    url: process.env.NEXT_PUBLIC_BASE_URL || 'https://xomcafe.io.vn',
  },
  robots: {
    index: true,
    follow: true,
    googleBot: {
      index: true,
      follow: true,
      'max-video-preview': -1,
      'max-image-preview': 'large',
      'max-snippet': -1,
    },
  },
  twitter: {
    card: 'summary_large_image',
    title: 'Xóm Cafe | Không gian cà phê thư giãn tại Hà Nội',
    description:
      'Thưởng thức cà phê chất lượng và đồ uống đặc sản trong không gian yên tĩnh tại Xóm Cafe',
    images: ['/logo-header-admin.png'],
  },
  alternates: {
    canonical: process.env.NEXT_PUBLIC_BASE_URL || 'https://xomcafe.io.vn',
  },
  authors: [{ name: 'Xóm Cafe' }],
  creator: 'Xóm Cafe',
  publisher: 'Xóm Cafe',
  // icons: {
  //   icon: '/favicon.ico',
  //   shortcut: '/favicon-16x16.png',
  //   apple: '/apple-touch-icon.png',
  // },
  category: 'cafe, coffee shop',
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="vi" suppressHydrationWarning>
      <Script id="schema-cafe" type="application/ld+json">
        {`
    {
      "@context": "https://schema.org",
      "@graph": [
        {
          "@type": "CafeOrCoffeeShop",
          "name": "Xóm Cafe - Cơ Sở Cầu Giấy",
          "image": "${
            process.env.NEXT_PUBLIC_BASE_URL || 'https://xomcafe.io.vn'
          }/logo-header-admin.png",
          "url": "${process.env.NEXT_PUBLIC_BASE_URL || 'https://xomcafe.io.vn'}",
          "address": {
            "@type": "PostalAddress",
            "streetAddress": "111 P. Nghĩa Tân",
            "addressLocality": "Nghĩa Tân, Cầu Giấy",
            "addressRegion": "Hà Nội",
            "addressCountry": "VN"
          },
          "telephone": "02435553456",
          "servesCuisine": "Cà phê Việt Nam",
          "priceRange": "$$",
          "openingHours": "Mo-Su 07:00-23:00",
          "paymentAccepted": "Cash, Credit Card",
          "priceSpecification": {
            "@type": "PriceSpecification",
            "price": "50000",
            "priceCurrency": "VND",
            "minPrice": "50000"
          },
          "parentOrganization": {
            "@type": "Organization",
            "name": "Xóm Cafe"
          }
        },
        {
          "@type": "CafeOrCoffeeShop",
          "name": "Xóm Cafe - Cơ Sở Thanh Xuân",
          "image": "${
            process.env.NEXT_PUBLIC_BASE_URL || 'https://xomcafe.io.vn'
          }/logo-header-admin.png",
          "url": "${process.env.NEXT_PUBLIC_BASE_URL || 'https://xomcafe.io.vn'}",
          "address": {
            "@type": "PostalAddress",
            "streetAddress": "N6E Khu đô thị Trung Hòa Nhân Chính",
            "addressLocality": "Thanh Xuân",
            "addressRegion": "Hà Nội",
            "addressCountry": "VN"
          },
          "telephone": "02435553456",
          "servesCuisine": "Cà phê Việt Nam",
          "priceRange": "$$",
          "openingHours": "Mo-Su 07:00-23:00",
          "paymentAccepted": "Cash, Credit Card",
          "priceSpecification": {
            "@type": "PriceSpecification",
            "price": "50000",
            "priceCurrency": "VND",
            "minPrice": "50000"
          },
          "parentOrganization": {
            "@type": "Organization",
            "name": "Xóm Cafe"
          }
        },
        {
          "@type": "CafeOrCoffeeShop",
          "name": "Xóm Cafe - Cơ Sở Nam Từ Liêm",
          "image": "${
            process.env.NEXT_PUBLIC_BASE_URL || 'https://xomcafe.io.vn'
          }/logo-header-admin.png",
          "url": "${process.env.NEXT_PUBLIC_BASE_URL || 'https://xomcafe.io.vn'}",
          "address": {
            "@type": "PostalAddress",
            "streetAddress": "A2 Phố Bùi Xuân Phái, Mỹ Đình",
            "addressLocality": "Nam Từ Liêm",
            "addressRegion": "Hà Nội",
            "addressCountry": "VN"
          },
          "telephone": "02435553456",
          "servesCuisine": "Cà phê Việt Nam",
          "priceRange": "$$",
          "openingHours": "Mo-Su 07:00-23:00",
          "paymentAccepted": "Cash, Credit Card",
          "priceSpecification": {
            "@type": "PriceSpecification",
            "price": "50000",
            "priceCurrency": "VND",
            "minPrice": "50000"
          },
          "parentOrganization": {
            "@type": "Organization",
            "name": "Xóm Cafe"
          }
        }
      ]
    }
  `}
      </Script>
      <body
        className={`${lobster.variable} ${vollkorn.variable} font-vollkorn antialiased bg-[#f8f5f2] dark:bg-black`}
      >
        <SessionProvider>
          <ReactQueryProvider>{children}</ReactQueryProvider>
        </SessionProvider>
        <Toaster />
      </body>
    </html>
  );
}
