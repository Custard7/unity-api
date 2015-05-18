import os
from setuptools import setup, find_packages

# Utility function to read the README file.
# Used for the long_description.  It's nice, because now 1) we have a top level
# README file and 2) it's easier to type in the README file than to put a raw
# string in below ...
def read(fname):
  return open(os.path.join(os.path.dirname(__file__), fname)).read()

setup(
  name = "unity_client",
  version = "0.0.1",
  author = "Chetan Surpur",
  author_email = "csurpur@numenta.com",
  description = ("Python client for Unity API."),
  packages=find_packages()
)
