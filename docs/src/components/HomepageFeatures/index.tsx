import type {ReactNode} from 'react';
import clsx from 'clsx';
import Heading from '@theme/Heading';
import styles from './styles.module.css';
import React from 'react';
import { Icon } from "@iconify/react";

type FeatureItem = {
  title: string;
  icon: string;
  Svg: React.ComponentType<React.ComponentProps<'svg'>>;
  description: ReactNode;
};

const FeatureList: FeatureItem[] = [
  {
    title: 'Federated Administration',
    icon: "ant-design:team-outlined",
    Svg: require('@site/static/img/undraw_docusaurus_mountain.svg').default,
    description: (
      <>
        hamSTER offers a detailed concept for roles and permissions, 
        allowing  efficient management of institutions, schools, faculties and departments 
      </>
    ),
  },
  {
    title: 'Modular Architecture',
    icon: "streamline:module-puzzle-3",
    Svg: require('@site/static/img/undraw_docusaurus_tree.svg').default,
    description: (
      <>
        hamSTER is built on a modular architecture, allowing using feature subsets. No all or nothing.
      </>
    ),
  },
  {
    title: 'Build for Sovereignty',
    icon: "ph:shipping-container-light",
    Svg: require('@site/static/img/undraw_docusaurus_react.svg').default,
    description: (
      <>
        hamSTER is build on services offering APIs for integration and extension. 
        The plattform is based on a containerized architecture using open source infrastructures.
      </>
    ),
  },
];

function Feature({title, icon, Svg, description}: FeatureItem) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center">
        <Icon icon={icon} style={{ fontSize: "72px" }}/>
      </div>
      <div className="text--center padding-horiz--md">
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures(): ReactNode {
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
