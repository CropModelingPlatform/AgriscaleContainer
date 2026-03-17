.. highlight:: shell

============
Contributing
============

Contributions are welcome, and they are greatly appreciated! Every little bit
helps, and credit will always be given.

You can contribute in many ways:

Types of Contributions
----------------------

Report Bugs
~~~~~~~~~~~

Report bugs at https://github.com/YOUR_USERNAME/AgriscaleContainer/issues.

If you are reporting a bug, please include:

* Your operating system name and version.
* Any details about your local setup that might be helpful in troubleshooting.
* Detailed steps to reproduce the bug.

Fix Bugs
~~~~~~~~

Look through the GitHub issues for bugs. Anything tagged with "bug" and "help
wanted" is open to whoever wants to implement it.

Implement Features
~~~~~~~~~~~~~~~~~~

Look through the GitHub issues for features. Anything tagged with "enhancement"
and "help wanted" is open to whoever wants to implement it.

Write Documentation
~~~~~~~~~~~~~~~~~~~

AgriscaleContainer could always use more documentation, whether as part of the
official docs, in docstrings, or even on the web in blog posts,
articles, and such.

Submit Feedback
~~~~~~~~~~~~~~~

The best way to send feedback is to file an issue at https://github.com/YOUR_USERNAME/AgriscaleContainer/issues.

If you are proposing a feature:

* Explain in detail how it would work.
* Keep the scope as narrow as possible, to make it easier to implement.
* Remember that this is a volunteer-driven project, and that contributions
  are welcome :)

Get Started!
------------

Ready to contribute? Here's how to set up AgriscaleContainer for local development.

1. Fork the AgriscaleContainer repo on GitHub.
2. Clone your fork locally::

    $ git clone git@github.com:your_name_here/AgriscaleContainer.git

3. Set up your development environment::

    $ cd AgriscaleContainer/
    # Ensure you have required dependencies:
    # - .NET SDK 5.0+ and 8.0
    # - Docker or Singularity
    # - gfortran, cmake, make

4. Create a branch for local development::

    $ git checkout -b name-of-your-bugfix-or-feature

   Now you can make your changes locally.

5. When you're done making changes, test your build::

    $ make clean
    $ make build        # Test building components
    $ make build_apsim  # Test APSIM if modified
    $ make build_dssat  # Test DSSAT if modified

6. Commit your changes and push your branch to GitHub::

    $ git add .
    $ git commit -m "Your detailed description of your changes."
    $ git push origin name-of-your-bugfix-or-feature

7. Submit a pull request through the GitHub website.

Pull Request Guidelines
-----------------------

Before you submit a pull request, check that it meets these guidelines:

1. The pull request should successfully build (test with ``make build``).
2. If the pull request adds functionality, the docs should be updated. Add the
   feature to the list in README.md.
3. Ensure your changes don't break existing functionality.
4. Follow the existing code style and conventions.

**Important Note on Third-Party Code:**

* Contributions should focus on the **container infrastructure** (Makefile, Dockerfile, build scripts, datamill, celsius).
* Do **NOT** include modifications to APSIM, DSSAT, or STICS source code in pull requests.
* Model-specific improvements should be contributed to the respective upstream projects:
  
  * APSIM: https://github.com/APSIMInitiative/ApsimX
  * DSSAT: https://github.com/DSSAT/dssat-csm-os
  * STICS: Contact INRAE directly

* Any contributed code becomes part of the MIT-licensed container infrastructure.
* Ensure you have the right to contribute any code you submit.

Tips
----

To test your changes quickly::

    $ make clean
    $ make build

To test specific components::

    $ make clean_apsim build_apsim
    $ make clean_dssat build_dssat
    $ make clean_celsius build_celsius

Deploying
---------

A reminder for the maintainers on how to deploy.
Make sure all your changes are committed (including an entry in HISTORY.rst if it exists).
Then run::

$ git tag -a v1.0.0 -m "Release version 1.0.0"
$ git push
$ git push --tags

GitHub Actions or manual release process will handle deployment.
