import clsx from "clsx";
import Heading from "@theme/Heading";
import styles from "./styles.module.css";

const FeatureList = [
    {
        title: "Cross Platform",
        Svg: require("@site/static/img/undraw_docusaurus_mountain.svg").default,
        description: (
            <>
                Designed from the ground up to be easily installed and run
                anywhere — Windows, macOS, or Linux — with just a few simple
                steps.
            </>
        ),
    },
    {
        title: "Easy to Use",
        Svg: require("@site/static/img/undraw_docusaurus_tree.svg").default,
        description: (
            <>
                Focus on your proof of concept or development tasks with an
                easy, intuitive command-line interface that allows you to send
                commands and publish events quickly.
            </>
        ),
    },
    {
        title: "Configurable",
        Svg: require("@site/static/img/undraw_docusaurus_react.svg").default,
        description: (
            <>
                Flexible configuration for multiple transports, with seamless
                switching when needed.
            </>
        ),
    },
];

function Feature({ Svg, title, description }) {
    return (
        <div className={clsx("col col--4")}>
            <div className="text--center">
                <Svg className={styles.featureSvg} role="img" />
            </div>
            <div className="text--center padding-horiz--md">
                <Heading as="h3">{title}</Heading>
                <p>{description}</p>
            </div>
        </div>
    );
}

export default function HomepageFeatures() {
    return (
        <section className={styles.features}>
            <div className="container">
                <div className="row">
                    {FeatureList.map((props, idx) => (
                        <Feature key={idx} {...props} />
                    ))}
                </div>
            </div>
        </section>
    );
}
